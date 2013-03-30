﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
using CQRS.Domain.Commands;
using CQRS.ReadModel;

namespace CQRS.WPFUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Session.VisitorId = Guid.NewGuid();
            visitorId.Text = Session.VisitorId.ToString();
            var visitorCmd = new CreateVisitorCommand(Session.VisitorId);
            BusFacade.MessageBus.Send(visitorCmd);
        }
    }
}