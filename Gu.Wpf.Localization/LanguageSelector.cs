﻿namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO.Packaging;
    using System.Linq;
    using System.Resources;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Threading;

    using Gu.Localization;

    [ContentProperty("Languages")]
    public class LanguageSelector : Control, IDisposable
    {
        public static readonly DependencyProperty AutogenerateLanguagesProperty = DependencyProperty.Register(
            "AutogenerateLanguages",
            typeof(bool),
            typeof(LanguageSelector),
            new PropertyMetadata(default(bool), OnAutoGenerateLanguagesChanged));

        private bool disposed = false;

        static LanguageSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LanguageSelector), new FrameworkPropertyMetadata(typeof(LanguageSelector)));
        }

        public LanguageSelector()
        {
            Translator.LanguageChanged += this.OnLanguageChanged;
            Translator.LanguagesChanged += this.OnLanguagesChanged;
            this.Languages = new ObservableCollection<Language>();
            this.Languages.CollectionChanged += (_, __) => this.UpdateSelected();
            this.UpdateSelected();
        }

        public bool AutogenerateLanguages
        {
            get { return (bool)this.GetValue(AutogenerateLanguagesProperty); }
            set { this.SetValue(AutogenerateLanguagesProperty, value); }
        }

        /// <summary>
        /// Gets gets or sets the cultures.
        /// </summary>
        public ObservableCollection<Language> Languages { get; }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Translator.LanguageChanged -= this.OnLanguageChanged;
            }
        }

        private static void OnAutoGenerateLanguagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LanguageSelector)d).OnLanguagesChanged(null, null);
        }

        private void OnLanguagesChanged(object _, EventArgs __)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(this.SyncLanguages));
        }

        private void OnLanguageChanged(object sender, CultureInfo e)
        {
            this.UpdateSelected();
        }

        private void UpdateSelected()
        {
            foreach (var language in this.Languages)
            {
                language.IsSelected = CultureInfoComparer.Equals(language.Culture, Translator.CurrentCulture);
            }
        }

        private void SyncLanguages()
        {
            if (this.AutogenerateLanguages)
            {
                for (int i = this.Languages.Count - 1; i >= 0; i--)
                {
                    if (!Translator.AllCultures.Contains(this.Languages[i].Culture, CultureInfoComparer.Default))
                    {
                        this.Languages.RemoveAt(i);
                    }
                }

                foreach (var cultureInfo in Translator.AllCultures)
                {
                    if (this.Languages.Any(x => CultureInfoComparer.Equals(x.Culture, cultureInfo)))
                    {
                        continue;
                    }

                    var language = new Language(cultureInfo);
                    var key = new Uri($"pack://application:,,,/{this.GetType().Assembly.GetName().Name};component/Flags/{cultureInfo.TwoLetterISOLanguageName}.png", UriKind.Absolute);
                    language.FlagSource = key;
                    this.Languages.Add(language);
                }

                this.UpdateSelected();
            }
        }
    }
}
