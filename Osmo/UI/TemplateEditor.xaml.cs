﻿using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for TemplateEditor.xaml
    /// </summary>
    public partial class TemplateEditor : Grid, IHotkeyHelper
    {
        private static TemplateEditor instance;

        private CompletionWindow completionWindow;

        public static TemplateEditor Instance
        {
            get
            {
                if (instance == null)
                    instance = new TemplateEditor();

                return instance;
            }
        }

        private TemplateEditor()
        {
            InitializeComponent();
        }

        public void LoadTemplate(ForumTemplate template)
        {
            (DataContext as TemplateEditorViewModel).Template = template;
            textEditor.Document = null; // immediately remove old document
            TextDocument doc = new TextDocument(new StringTextSource(template.Content));
            doc.SetOwnerThread(Application.Current.Dispatcher.Thread);
            Application.Current.Dispatcher.BeginInvoke(
                  new Action(
                      delegate
                      {
                          textEditor.Document = doc;
                      }), DispatcherPriority.Normal);

        }

        public void SaveTemplate()
        {
            (DataContext as TemplateEditorViewModel).Template.Save(textEditor.Text);
            snackbar.MessageQueue.Enqueue(Helper.FindString("snackbar_templateEditor_saveText"));
        }

        private void TextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            completionWindow = new CompletionWindow(textEditor.TextArea);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            foreach (CompletionData item in FixedValues.templateCompletionData)
            {
                if (item.Text.Contains(e.Text.ToUpper()))
                    data.Add(item);
            }

            if (data.Count > 0)
            {
                completionWindow.Show();
                completionWindow.Closed += delegate { completionWindow = null; };
            }
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (completionWindow != null)
            {
                // Whenever a non-letter is typed while the completion window is open,
                // insert the currently selected element.
                completionWindow.CompletionList.RequestInsertion(e);
            }
        }

        public void MakePreview(Skin skin)
        {
            TemplateEditorViewModel vm = DataContext as TemplateEditorViewModel;
            vm.TargetSkin = skin;
            vm.PreviewText = Helper.ApplyForumTemplate(textEditor.Text, vm.TargetSkin);
        }

        public bool ForwardKeyboardInput(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.P:
                        e.Handled = true;
                        if (DialogHost.OpenDialogCommand.CanExecute(btn_preview.CommandParameter, btn_preview))
                            DialogHost.OpenDialogCommand.Execute(btn_preview.CommandParameter, btn_preview);
                        break;
                    case Key.S:
                        e.Handled = true;
                        SaveTemplate();
                        break;
                }
            }

            return e.Handled;
        }

        private void Grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ForwardKeyboardInput(e);
        }
    }
}
