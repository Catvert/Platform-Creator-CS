using System;
using System.Numerics;
using ImGuiNET;

namespace Platform_Creator_CS.Utility {
    public static class ImGuiHelper {
        public static void WithCenteredWindow(string windowTitle, Vector2 windowSize, Condition centerCond, WindowFlags flags, Action block) {
            ImGui.SetNextWindowSize(windowSize, centerCond);
            ImGui.SetNextWindowPos(new Vector2(PCGame.ScreenSize.X / 2f - windowSize.X / 2f, PCGame.ScreenSize.Y / 2f - windowSize.Y / 2f), centerCond, new Vector2());
            
            ImGui.BeginWindow(windowTitle, flags);
            
            block();
            
            ImGui.EndWindow();
        }
        public static void WithMenuWindow(Vector2 windowSize, Action block) {
            ImGui.PushStyleColor(ColorTarget.WindowBg, new Vector4(0f));
            ImGui.PushStyleColor(ColorTarget.Button, new Vector4(0.2f, 0.5f, 0.9f, 1f));
            ImGui.PushStyleColor(ColorTarget.Text, new Vector4(1f));
            ImGui.PushStyleColor(ColorTarget.Border, new Vector4(0f));
            ImGui.PushStyleVar(StyleVar.FrameRounding, 10f);
            ImGui.PushStyleVar(StyleVar.FramePadding, new Vector2(10f));
            ImGui.PushFont(PCGame.ImGuiBigFont);

            WithCenteredWindow("#menu", windowSize, Condition.Always, WindowFlags.NoTitleBar | WindowFlags.NoCollapse | WindowFlags.NoMove | WindowFlags.NoResize | WindowFlags.NoBringToFrontOnFocus, block);
            
            ImGui.PopStyleColor(4);
            ImGui.PopStyleVar(2);
            ImGui.PopFont();
        }
    }
}