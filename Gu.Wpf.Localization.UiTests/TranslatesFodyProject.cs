namespace Gu.Wpf.Localization.UiTests
{
    using System.Linq;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class TranslatesFodyProject
    {
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched("Gu.Wpf.Localization.Demo.Fody.exe");
        }

        [Test]
        public void DetectsLanguages()
        {
            using (var application = Application.AttachOrLaunch("Gu.Wpf.Localization.Demo.Fody.exe"))
            {
                var window = application.MainWindow;
                var comboBox = window.FindComboBox("LanguageComboBox");
                var expected = new[] { "English (United Kingdom)", "Nederlands (Nederland)", "svenska (Sverige)" };
                var actual = comboBox.Items.Select(x => x.Text).ToArray();
                CollectionAssert.AreEquivalent(expected, actual);
            }
        }

        [Test]
        public void Translates()
        {
            using (var application = Application.AttachOrLaunch("Gu.Wpf.Localization.Demo.Fody.exe"))
            {
                var window = application.MainWindow;
                var comboBox = window.FindComboBox("LanguageComboBox");
                var label = window.FindLabel("TranslatedLabel");

                comboBox.Select("English (United Kingdom)");
                Assert.AreEqual("English", label.Text);

                comboBox.Select("Nederlands (Nederland)");
                Assert.AreEqual("Nederlands", label.Text);

                comboBox.Select("svenska (Sverige)");
                Assert.AreEqual("Svenska", label.Text);
            }
        }
    }
}
