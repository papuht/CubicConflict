using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionDrawer {

    public Texture2D selectionTexture;

    public SelectionDrawer() {
        this.selectionTexture = new Texture2D(1, 1);
        selectionTexture.Apply();
    }

    public SelectionDrawer(Texture2D t) {
        this.selectionTexture = t;
        this.selectionTexture.Apply();
    }

    private void draw(Rect rect, Color color) {
        GUI.color = color;
        GUI.DrawTexture(rect, this.selectionTexture);
        GUI.color = Color.white;
    }

    //Only Call this onGUI
    public void drawRectangle(Vector2 start, Vector2 stop, Color color) {
       this.draw(this.getDrawableRect(start, stop), color);
    }

    public void DrawRectangleBorders(Vector2 start, Vector2 stop, float width, Color color) {
        Rect rect = this.getDrawableRect(start, stop);
        this.draw(new Rect(rect.xMin, rect.yMin, rect.width, width), color); //Top border
        this.draw(new Rect(rect.xMin, rect.yMax - width, rect.width, width), color); //Bottom border
        this.draw(new Rect(rect.xMin, rect.yMin, width, rect.height), color); //Left border
        this.draw(new Rect(rect.xMax - width, rect.yMin, width, rect.height), color); //Right border
        
    }

    //Gui.Draw asummes coordinates start form top left corner, we need to translate them
    private Rect getDrawableRect(Vector2 start, Vector2 stop) { 

        //Translate heights to start from top left
        start.y = Screen.height - start.y;
        stop.y = Screen.height - stop.y;

        //Match the smallest and largest cordinate values together
        Vector2 guiStart = Vector2.Min(start, stop);
        Vector2 guiStop = Vector2.Max(start, stop);

        return Rect.MinMaxRect(guiStart.x, guiStart.y, guiStop.x, guiStop.y);
    }
}