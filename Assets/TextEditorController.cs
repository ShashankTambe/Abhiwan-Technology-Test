using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TextEditorController : MonoBehaviour
{
    public TMP_InputField inputField;           // Assign this in the inspector
    public Button boldButton, italicButton, undoButton, redoButton;

    private Stack<string> undoStack = new Stack<string>();  // Stores previous states for undo
    private Stack<string> redoStack = new Stack<string>();  // Stores states for redo

    private bool isBold = false;
    private bool isItalic = false;

    void Start()
    {
        // Register button click listeners
        boldButton.onClick.AddListener(ToggleBold);
        italicButton.onClick.AddListener(ToggleItalic);
        undoButton.onClick.AddListener(Undo);
        redoButton.onClick.AddListener(Redo);

        // Capture initial state in the undo stack
        undoStack.Push(inputField.text);
    }

    // Toggle Bold
    private void ToggleBold()
    {
        isBold = !isBold;
        UpdateTextStyle();
    }

    // Toggle Italic
    private void ToggleItalic()
    {
        isItalic = !isItalic;
        UpdateTextStyle();
    }

    // Apply Bold and Italic to the InputField text
    private void UpdateTextStyle()
    {
        string stylePrefix = "<b><i>";
        string styleSuffix = "</i></b>";

        // Add tags based on the selected styles
        if (isBold && isItalic)
        {
            inputField.text = stylePrefix + inputField.text + styleSuffix;
        }
        else if (isBold)
        {
            inputField.text = "<b>" + inputField.text + "</b>";
        }
        else if (isItalic)
        {
            inputField.text = "<i>" + inputField.text + "</i>";
        }
        else
        {
            // Reset to plain text if no styles are selected
            inputField.text = inputField.text.Replace("<b>", "").Replace("</b>", "").Replace("<i>", "").Replace("</i>", "");
        }

        SaveStateForUndo();
    }

    // Undo the last action
    private void Undo()
    {
        if (undoStack.Count > 1)  // Ensure there is something to undo
        {
            string currentText = undoStack.Pop();
            redoStack.Push(currentText);
            inputField.text = undoStack.Peek();  // Set the text to the last saved state
        }
    }

    // Redo the last undone action
    private void Redo()
    {
        if (redoStack.Count > 0)  // Ensure there is something to redo
        {
            string redoText = redoStack.Pop();
            undoStack.Push(redoText);
            inputField.text = redoText;
        }
    }

    // Save the current state to the undo stack
    private void SaveStateForUndo()
    {
        // Only add to undo stack if text has changed
        if (undoStack.Count == 0 || inputField.text != undoStack.Peek())
        {
            undoStack.Push(inputField.text);
            redoStack.Clear();  // Clear redo stack on new input
        }
    }

    // Register text change to save new state
    public void OnTextChanged()
    {
        SaveStateForUndo();
    }
}
