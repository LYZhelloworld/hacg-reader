// <copyright file="FetchButtonViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    using System;
    using System.Windows.Input;
    using HAcgReader.Resources;
    using Microsoft.Toolkit.Mvvm.Input;

    /// <summary>
    /// 拉取按钮视图模型
    /// </summary>
    public class FetchButtonViewModel : BaseViewModel
    {
        /// <summary>
        /// 拉取命令
        /// </summary>
        private readonly RelayCommand command;

        /// <summary>
        /// 拉取按钮是否有效
        /// </summary>
        private bool isEnabled = true;

        /// <summary>
        /// 是否正在拉取
        /// </summary>
        private bool isFetching;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FetchButtonViewModel()
        {
            this.command = new RelayCommand(this.Execute);
        }

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        public event EventHandler? Started;

        /// <summary>
        /// 按钮取消事件
        /// </summary>
        public event EventHandler? Cancelled;

        /// <summary>
        /// 拉取按钮是否有效
        /// </summary>
        public bool IsEnabled
        {
            get => this.isEnabled;
            set
            {
                this.isEnabled = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否正在拉取
        /// </summary>
        public bool IsFetching
        {
            get => this.isFetching;
            set
            {
                this.isFetching = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.ButtonText));
            }
        }

        /// <summary>
        /// 按钮文本
        /// </summary>
        public string ButtonText => this.IsFetching ? Strings.FetchButtonTextFetching : Strings.FetchButtonTextNotFetching;

        /// <summary>
        /// 拉取命令
        /// </summary>
        public ICommand Command => this.command;

        /// <summary>
        /// 按钮点击
        /// </summary>
        public void Execute()
        {
            if (this.IsEnabled)
            {
                if (this.IsFetching)
                {
                    this.Cancelled?.Invoke(this, EventArgs.Empty);
                    this.IsFetching = false;
                }
                else
                {
                    this.IsFetching = true;
                    this.Started?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}