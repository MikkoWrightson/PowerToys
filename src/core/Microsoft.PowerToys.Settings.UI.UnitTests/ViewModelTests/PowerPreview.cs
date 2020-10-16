﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text.Json;
using Microsoft.PowerToys.Settings.UI.Lib;
using Microsoft.PowerToys.Settings.UI.Lib.ViewModels;
using Microsoft.PowerToys.Settings.UI.UnitTests.BackwardsCompatibility;
using Microsoft.PowerToys.Settings.UI.UnitTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ViewModelTests
{
    [TestClass]
    public class PowerPreview
    {

        private Mock<ISettingsUtils> mockPowerPreviewSettingsUtils;

        [TestInitialize]
        public void SetUpStubSettingUtils()
        {
            mockPowerPreviewSettingsUtils = ISettingsUtilsMocks.GetStubSettingsUtils<PowerPreviewSettings>();
        }

        /// <summary>
        /// Test if the original settings files were modified.
        /// </summary>
        [TestMethod]
        [DataRow("v0.18.2", "settings.json")]
        [DataRow("v0.19.2", "settings.json")]
        [DataRow("v0.20.1", "settings.json")]
        [DataRow("v0.21.1", "settings.json")]
        [DataRow("v0.22.0", "settings.json")]
        public void OriginalFilesModificationTest(string version, string fileName)
        {
            var mockIOProvider = BackCompatTestProperties.GetModuleIOProvider(version, PowerPreviewSettings.ModuleName, fileName);
            var mockSettingsUtils = new SettingsUtils(mockIOProvider.Object);
            PowerPreviewSettings originalSettings = mockSettingsUtils.GetSettings<PowerPreviewSettings>(PowerPreviewSettings.ModuleName);
            var repository = new BackCompatTestProperties.MockSettingsRepository<PowerPreviewSettings>(mockSettingsUtils);

            // Initialise View Model with test Config files
            Func<string, int> SendMockIPCConfigMSG = msg => { return 0; };
            PowerPreviewViewModel viewModel = new PowerPreviewViewModel(repository, SendMockIPCConfigMSG);

            // Verifiy that the old settings persisted
            Assert.AreEqual(originalSettings.Properties.EnableMdPreview, viewModel.MDRenderIsEnabled);
            Assert.AreEqual(originalSettings.Properties.EnableSvgPreview, viewModel.SVGRenderIsEnabled);
            Assert.AreEqual(originalSettings.Properties.EnableSvgThumbnail, viewModel.SVGThumbnailIsEnabled);

            //Verify that the stub file was used
            var expectedCallCount = 2;  //once via the view model, and once by the test (GetSettings<T>)
            BackCompatTestProperties.VerifyModuleIOProviderWasRead(mockIOProvider, PowerPreviewSettings.ModuleName, expectedCallCount);
        }


        [TestMethod]
        public void SVGRenderIsEnabledShouldPrevHandlerWhenSuccessful()
        {
            // Assert
            Func<string, int> SendMockIPCConfigMSG = msg =>
            {
                SndModuleSettings<SndPowerPreviewSettings> snd = JsonSerializer.Deserialize<SndModuleSettings<SndPowerPreviewSettings>>(msg);
                Assert.IsTrue(snd.PowertoysSetting.FileExplorerPreviewSettings.Properties.EnableSvgPreview);
                return 0;
            };

            // arrange
            PowerPreviewViewModel viewModel = new PowerPreviewViewModel(SettingsRepository<PowerPreviewSettings>.GetInstance(mockPowerPreviewSettingsUtils.Object), SendMockIPCConfigMSG, PowerPreviewSettings.ModuleName);

            // act
            viewModel.SVGRenderIsEnabled = true;
        }

        [TestMethod]
        public void SVGThumbnailIsEnabledShouldPrevHandlerWhenSuccessful()
        {
            // Assert
            Func<string, int> SendMockIPCConfigMSG = msg =>
            {
                SndModuleSettings<SndPowerPreviewSettings> snd = JsonSerializer.Deserialize<SndModuleSettings<SndPowerPreviewSettings>>(msg);
                Assert.IsTrue(snd.PowertoysSetting.FileExplorerPreviewSettings.Properties.EnableSvgThumbnail);
                return 0;
            };

            // arrange
            PowerPreviewViewModel viewModel = new PowerPreviewViewModel(SettingsRepository<PowerPreviewSettings>.GetInstance(mockPowerPreviewSettingsUtils.Object), SendMockIPCConfigMSG, PowerPreviewSettings.ModuleName);

            // act
            viewModel.SVGThumbnailIsEnabled = true;
        }

        [TestMethod]
        public void MDRenderIsEnabledShouldPrevHandlerWhenSuccessful()
        {
            // Assert
            Func<string, int> SendMockIPCConfigMSG = msg =>
            {
                SndModuleSettings<SndPowerPreviewSettings> snd = JsonSerializer.Deserialize<SndModuleSettings<SndPowerPreviewSettings>>(msg);
                Assert.IsTrue(snd.PowertoysSetting.FileExplorerPreviewSettings.Properties.EnableMdPreview);
                return 0;
            };

            // arrange
            PowerPreviewViewModel viewModel = new PowerPreviewViewModel(SettingsRepository<PowerPreviewSettings>.GetInstance(mockPowerPreviewSettingsUtils.Object), SendMockIPCConfigMSG, PowerPreviewSettings.ModuleName);

            // act
            viewModel.MDRenderIsEnabled = true;
        }
    }
}