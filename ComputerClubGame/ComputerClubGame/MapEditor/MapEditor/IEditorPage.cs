using System;
namespace MapEditor
{
    interface IEditorPage
    {
        bool CloseEditor();
        void OpenEditor(string file);
        string OpenFilePath { get; set; }
        bool SaveEditor();
        string SaveFilePath { get; set; }
        string Title { get; }
        event EventHandler TitleChanged;
        string TitleName { get; }
    }
}
