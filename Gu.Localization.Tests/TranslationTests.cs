﻿// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.Localization.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using NUnit.Framework;

    public class TranslationTests
    {
        [Test]
        public void GetOrCreateResourceManagerAndKeyCaches1()
        {
            Translator.CurrentCulture = new CultureInfo("sv");
            var translation1 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            var translation2 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            Assert.AreSame(translation1, translation2);
        }

        [Test]
        public void GetOrCreateResourceManagerAndKeyCaches2()
        {
            Translator.CurrentCulture = new CultureInfo("sv");
            var translation1 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), ErrorHandling.ReturnErrorInfo);
            var translation2 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), ErrorHandling.ReturnErrorInfo);
            Assert.AreSame(translation1, translation2);
            Assert.AreEqual(ErrorHandling.ReturnErrorInfo, translation1.ErrorHandling);

            var translation3 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), ErrorHandling.Inherit);
            Assert.AreNotSame(translation1, translation3);
            Assert.AreEqual(ErrorHandling.Inherit, translation3.ErrorHandling);

            var translation4 = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages), ErrorHandling.Throw);
            Assert.AreNotSame(translation1, translation4);
            Assert.AreNotSame(translation3, translation4);
            Assert.AreEqual(ErrorHandling.Throw, translation4.ErrorHandling);
        }

        [Test]
        public void GetOrCreateResourceManagerAndKeyThrowsForMissing()
        {
            Translator.CurrentCulture = new CultureInfo("sv");
            var exception = Assert.Throws<ArgumentOutOfRangeException>(()=> Translation.GetOrCreate(Properties.Resources.ResourceManager, "Missing"));
            var expected = "The resourcemanager: Gu.Localization.Tests.Properties.Resources does not have the key: Missing\r\n" +
                           "Parameter name: key";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void NotifiesAndTranslatesWhenLanguageChanges()
        {
            Translator.CurrentCulture = new CultureInfo("sv");
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.AllLanguages));
            var changes = new List<string>();
            translation.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

            Translator.CurrentCulture = new CultureInfo("en");
            Assert.AreEqual("English", translation.Translated);
            CollectionAssert.AreEqual(new[] { nameof(Translation.Translated) }, changes);

            Translator.CurrentCulture = new CultureInfo("sv");
            Assert.AreEqual("Svenska", translation.Translated);
            CollectionAssert.AreEqual(new[] { nameof(Translation.Translated), nameof(Translation.Translated) }, changes);
        }

        [TestCase(nameof(Properties.Resources.AllLanguages), "en", "English")]
        [TestCase(nameof(Properties.Resources.AllLanguages), "sv", "Svenska")]
        [TestCase(nameof(Properties.Resources.EnglishOnly), "sv", "_EnglishOnly_")]
        public void Translate(string key, string culture, string expected)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            var translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, key, ErrorHandling.ReturnErrorInfo);
            Translator.CurrentCulture = cultureInfo;
            var actual = translation.Translated;
            Assert.AreEqual(expected, actual);
        }
    }
}
