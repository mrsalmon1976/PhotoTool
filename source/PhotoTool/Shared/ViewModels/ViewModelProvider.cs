﻿using CommunityToolkit.Mvvm.DependencyInjection;
using ReactiveUI;

namespace PhotoTool.Shared.ViewModels
{
    public interface IViewModelProvider
    {
        T GetViewModel<T>() where T : ReactiveObject;
    }

    public class ViewModelProvider : IViewModelProvider
    {
        public T GetViewModel<T>() where T : ReactiveObject
        {
            // we have to use the locator pattern here
            return Ioc.Default.GetRequiredService<T>();
        }
    }
}
