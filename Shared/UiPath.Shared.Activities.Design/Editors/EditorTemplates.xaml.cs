﻿using System.Windows;

namespace Indico.UiPath.Shared.Activities.Design.Editors
{
    public partial class EditorTemplates
    {
        static ResourceDictionary resources;

        internal static ResourceDictionary ResourceDictionary
        {
            get
            {
                if (resources == null)
                {
                    resources = new EditorTemplates();
                }

                return resources;
            }
        }

        public EditorTemplates()
        {
            InitializeComponent();
        }
    }
}
