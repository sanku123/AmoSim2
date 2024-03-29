﻿using AmoSim2.Player;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmoSim2.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<PlayerViewModel>();
            SimpleIoc.Default.Register<SimulationViewModel>();

        }

        private static ViewModelLocator _instance = null;

        public static ViewModelLocator Instance
        {
            get
            {
                if (_instance == null) _instance = new ViewModelLocator();
                return _instance;
            }
        }

        public PlayerViewModel PlayerViewModel => ServiceLocator.Current.GetInstance<PlayerViewModel>();
        public SimulationViewModel SimulationViewModel => ServiceLocator.Current.GetInstance<SimulationViewModel>();


    }
}