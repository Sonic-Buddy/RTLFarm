﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTLFarm.Views.DialogPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterAccountDialog : Rg.Plugins.Popup.Pages.PopupPage
    {
        public RegisterAccountDialog()
        {
            InitializeComponent();
        }
    }
}