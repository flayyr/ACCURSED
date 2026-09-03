using System;
using SaintsField.Editor.Utils.WaitableUtils;
using SaintsField.Utils;
using UnityEditor;
using UnityEngine;

namespace SaintsField.Editor.Utils
{
    public static class IMGUIUtils
    {
        public readonly struct TickWaiterResult
        {
            public readonly Waiter.MoveNextStatus Result;
            public readonly Exception Exception;
            public readonly float Process;

            public readonly Type ResultType;
            public readonly object ResultValue;

            public TickWaiterResult(Waiter.MoveNextStatus result, Exception exception, float process, Type resultType, object resultValue)
            {
                Result = result;
                Exception = exception;
                Process = process;

                ResultType = resultType;
                ResultValue = resultValue;
            }
        }

        public class IMGUITicker : Util.ITicker
        {
            private const float PaddingWidthIMGUI = 3f;
            private const float StatusSizeIMGUI = 14f;
            private const float CloseButtonWidthIMGUI = 18f;
            private const float StatusDurationIMGUI = 2f;
            private const string CloseButtonIconIMGUI = "close.png";
            private const string StatusErrorIconIMGUI = "close.png";
            private const string StatusErrorColorIMGUI = "#FF2D17";
            private const string StatusPauseIconIMGUI = "d_PauseButton";
            private const string StatusPauseColorIMGUI = "#9717FF";

            private static readonly System.Collections.Generic.Dictionary<string, Texture2D> StatusIconCacheIMGUI =
                new System.Collections.Generic.Dictionary<string, Texture2D>();

            public TickWaiterResult TickWaiterResult;
            public bool Resolved { get; private set; }
            public bool IsRunning()
            {
                return _waiter != null;
            }

            private Waiter _waiter;
            private Action<object> _succeedCallback;
            private readonly IMGUILoading _loading = new IMGUILoading();
            private Waiter.MoveNextStatus _status;
            private double _statusHideAt = -1d;

            public void StartTrack(Waiter waiter, Action<object> succeedCallback)
            {
                Resolved = false;
                _waiter = waiter;
                _succeedCallback = succeedCallback;
                _status = Waiter.MoveNextStatus.Pending;
                _statusHideAt = -1d;
            }

            public void ResetResolved()
            {
                Resolved = false;
                _status = default;
                _statusHideAt = -1d;
            }

            public bool DropdownButton(Rect position, GUIContent content, GUIStyle style)
            {
                bool isRunning = IsRunning();
                Rect buttonRect = isRunning
                    ? new Rect(position)
                    {
                        width = Mathf.Max(0f, position.width - CloseButtonWidthIMGUI),
                    }
                    : position;

                bool clicked;
                using (new EditorGUI.DisabledScope(isRunning))
                {
                    clicked = GUI.Button(buttonRect, content, style);
                }

                if (isRunning)
                {
                    Rect closeRect = new Rect(position)
                    {
                        x = position.xMax - CloseButtonWidthIMGUI,
                        width = CloseButtonWidthIMGUI,
                    };
                    GUIContent closeContent = new GUIContent(GetStatusIconIMGUI(CloseButtonIconIMGUI));
                    if (GUI.Button(closeRect, closeContent, EditorStyles.miniButton))
                    {
                        _status = Waiter.MoveNextStatus.Cancelled;
                        _statusHideAt = EditorApplication.timeSinceStartup + StatusDurationIMGUI;
                        Resolved = true;
                        Reset();
                    }
                }

                DrawStatusIMGUI(buttonRect);
                return clicked;
            }

            public void DrawLoading(Rect position)
            {
                if (!IsRunning())
                {
                    return;
                }

                DrawStatusIMGUI(position);
            }

            public void Tick()
            {
                if (_waiter == null || Resolved)
                {
                    return;
                }

                TickWaiterResult tickResult = TickWaiter(_waiter);
                TickWaiterResult = tickResult;
                _status = tickResult.Result;
                switch (tickResult.Result)
                {
                    case Waiter.MoveNextStatus.Pending:
                        return;
                    case Waiter.MoveNextStatus.Completed:
                        _succeedCallback.Invoke(tickResult.ResultValue);
                        Resolved = true;
                        _status = default;
                        _statusHideAt = -1d;
                        Reset();
                        return;
                    case Waiter.MoveNextStatus.Cancelled:
                        Resolved = true;
                        _statusHideAt = EditorApplication.timeSinceStartup + StatusDurationIMGUI;
                        Reset();
                        return;
                    case Waiter.MoveNextStatus.Faulted:
                        Resolved = true;
                        _statusHideAt = EditorApplication.timeSinceStartup + StatusDurationIMGUI;
                        Reset();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(tickResult.Result), tickResult.Result, null);
                }
            }

            private void Reset()
            {
                _waiter = null;
                _succeedCallback = null;
            }

            private void DrawStatusIMGUI(Rect position)
            {
                if (_statusHideAt > 0d && EditorApplication.timeSinceStartup > _statusHideAt)
                {
                    _status = default;
                    _statusHideAt = -1d;
                    return;
                }

                Rect statusRect = new Rect(position)
                {
                    x = position.x + PaddingWidthIMGUI,
                    y = position.y + (position.height - StatusSizeIMGUI) / 2f,
                    width = StatusSizeIMGUI,
                    height = StatusSizeIMGUI,
                };

                switch (_status)
                {
                    case Waiter.MoveNextStatus.Pending:
                        if (!IsRunning())
                        {
                            return;
                        }
                        _loading.Draw(statusRect);
                        DrawProgressIMGUI(position, TickWaiterResult.Process);
                        return;
                    case Waiter.MoveNextStatus.Cancelled:
                        DrawStatusIconIMGUI(statusRect, StatusPauseIconIMGUI, StatusPauseColorIMGUI);
                        return;
                    case Waiter.MoveNextStatus.Faulted:
                        DrawStatusIconIMGUI(statusRect, StatusErrorIconIMGUI, StatusErrorColorIMGUI);
                        return;
                    case Waiter.MoveNextStatus.Completed:
                    default:
                        return;
                }
            }

            private static void DrawProgressIMGUI(Rect position, float progress)
            {
                if (progress < 0f)
                {
                    return;
                }

                Rect progressBackRect = new Rect(position)
                {
                    x = position.x + 2f,
                    y = position.yMax - 2f,
                    width = Mathf.Max(0f, position.width - 4f),
                    height = 1f,
                };
                EditorGUI.DrawRect(progressBackRect, new Color(0f, 0f, 0f, 0.25f));

                Rect progressRect = new Rect(progressBackRect)
                {
                    width = progressBackRect.width * Mathf.Clamp01(progress),
                };
                EditorGUI.DrawRect(progressRect, new Color(0f, 182f / 255f, 1f, 0.75f));
            }

            private static void DrawStatusIconIMGUI(Rect texRect, string iconName, string iconColor)
            {
                Texture2D texture = GetStatusIconIMGUI(iconName);
                if (texture == null)
                {
                    return;
                }

                using (new GUIColorScoop(Colors.GetColorByStringPresent(iconColor)))
                {
                    GUI.DrawTexture(texRect, texture, ScaleMode.ScaleToFit, true);
                }
            }

            private static Texture2D GetStatusIconIMGUI(string iconName)
            {
                if (StatusIconCacheIMGUI.TryGetValue(iconName, out Texture2D texture) && texture != null)
                {
                    return texture;
                }

                texture = Util.LoadResource<Texture2D>(iconName);
                if (texture != null)
                {
                    StatusIconCacheIMGUI[iconName] = texture;
                }

                return texture;
            }

            private static TickWaiterResult TickWaiter(Waiter waiter)
            {
                waiter.Update();
                if (!waiter.SubWaiterDone())
                {
                    return new TickWaiterResult(
                        Waiter.MoveNextStatus.Pending,
                        null,
                        waiter.GetProgress(),
                        null,
                        null
                    );
                }

                Waiter.MoveNextResult moveNext = waiter.MoveNext();

                if (moveNext.Exception != null)
                {
                    return new TickWaiterResult(
                        Waiter.MoveNextStatus.Faulted,
                        moveNext.Exception,
                        waiter.GetProgress(),
                        null,
                        null
                    );
                }

                switch (moveNext.Status)
                {
                    case Waiter.MoveNextStatus.Pending:
                        waiter.CheckCurrentNeedWaiter();
                        return new TickWaiterResult(
                            Waiter.MoveNextStatus.Pending,
                            null,
                            waiter.GetProgress(),
                            null,
                            null
                        );
                    case Waiter.MoveNextStatus.Completed:
                        return new TickWaiterResult(
                            Waiter.MoveNextStatus.Completed,
                            null,
                            waiter.GetProgress(),
                            moveNext.ReturnType,
                            moveNext.ReturnValue
                        );
                    case Waiter.MoveNextStatus.Cancelled:
                        return new TickWaiterResult(
                            Waiter.MoveNextStatus.Cancelled,
                            null,
                            waiter.GetProgress(),
                            moveNext.ReturnType,
                            moveNext.ReturnValue
                        );
                    case Waiter.MoveNextStatus.Faulted:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(moveNext.Status), moveNext.Status, null);
                }
            }
        }
    }
}
