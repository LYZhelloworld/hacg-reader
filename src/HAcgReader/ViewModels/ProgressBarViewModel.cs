// <copyright file="ProgressBarViewModel.cs" company="Helloworld">
// Copyright (c) Helloworld. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HAcgReader.ViewModels
{
    /// <summary>
    /// 滚动条视图模型
    /// </summary>
    public class ProgressBarViewModel : BaseViewModel
    {
        /// <summary>
        /// 滚动条当前值
        /// </summary>
        private int value;

        /// <summary>
        /// 滚动条最大值
        /// </summary>
        private int maximum = 10;

        /// <summary>
        /// 滚动条处于不确定状态
        /// </summary>
        private bool isIndeterminate;

        /// <summary>
        /// 滚动条当前值
        /// </summary>
        public int Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 滚动条最大值
        /// </summary>
        public int Maximum
        {
            get => this.maximum;
            set
            {
                this.maximum = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 滚动条处于不确定状态
        /// </summary>
        public bool IsIndeterminate
        {
            get => this.isIndeterminate;
            set
            {
                this.isIndeterminate = value;
                this.OnPropertyChanged();
            }
        }
    }
}
