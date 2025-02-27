﻿using HomeSpeaker.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeSpeaker.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StreamPage : ContentPage
    {
        public StreamPage()
        {
            InitializeComponent();
            BindingContext = new StreamViewModel();
            NewProp = true;
        }

        public bool NewProp { get; private set; }
    }
}