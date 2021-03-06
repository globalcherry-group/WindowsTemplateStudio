﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Templates.Core;
using Microsoft.Templates.Core.Gen;
using Microsoft.TemplateEngine.Abstractions;

using Xunit;
using Microsoft.Templates.Fakes;

namespace Microsoft.Templates.Test
{
    [Collection("GenerationCollection")]
    [Trait("ExecutionSet", "Generation")]
    public class GenProjectTests : BaseGenAndBuildTests
    {
        public GenProjectTests(GenerationFixture fixture)
            : base(fixture)
        {
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForGenerationAsync))]
        [Trait("Type", "GenerationProjects")]
        public async Task GenEmptyProjectAsync(string projectType, string framework, string platform, string language)
        {
            Func<ITemplateInfo, bool> selector =
                t => t.GetTemplateType() == TemplateType.Project
                    && t.GetProjectTypeList().Contains(projectType)
                    && t.GetFrameworkList().Contains(framework)
                    && t.GetPlatform() == platform
                    && !t.GetIsHidden()
                    && t.GetLanguage() == language;

            var projectName = $"{projectType}{framework}";

            await AssertGenerateProjectAsync(selector, projectName, projectType, framework, platform, language);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForGenerationAsync))]
        [Trait("Type", "GenerationProjects")]
        public async Task GenEmptyProjectCorrectInferProjectConfigInfoAsync(string projectType, string framework, string platform, string language)
        {
            Func<ITemplateInfo, bool> selector =
                t => t.GetTemplateType() == TemplateType.Project
                    && t.GetProjectTypeList().Contains(projectType)
                    && t.GetFrameworkList().Contains(framework)
                    && t.GetPlatform() == platform
                    && !t.GetIsHidden()
                    && t.GetLanguage() == language;

            var projectName = $"{projectType}{framework}";

            string projectPath = await AssertGenerateProjectAsync(selector, projectName, projectType, framework, platform, language, null, null, false);

            // Remove configuration from the manifest
            RemoveProjectConfigInfoFromProject();

            // Now the configuration must be inferred and should be as expected
            AssertCorrectProjectConfigInfo(projectType, framework, platform);

            AssertProjectConfigInfoRecreated(projectType, framework, platform);
        }

        private static void AssertProjectConfigInfoRecreated(string projectType, string framework, string platform)
        {
            string content = File.ReadAllText(Path.Combine(GenContext.Current.DestinationPath, "Package.appxmanifest"));
            string expectedFxText = $"Name=\"framework\" Value=\"{framework}\"";
            string expectedPtText = $"Name=\"projectType\" Value=\"{projectType}\"";
            string expectedPfText = $"Name=\"platform\" Value=\"{platform}\"";

            Assert.Contains(expectedFxText, content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(expectedPtText, content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(expectedPfText, content, StringComparison.OrdinalIgnoreCase);
        }

        private void RemoveProjectConfigInfoFromProject()
        {
            string manifest = Path.Combine(GenContext.Current.DestinationPath, "Package.appxmanifest");
            var lines = File.ReadAllLines(manifest);
            StringBuilder sb = new StringBuilder();
            string fx = $"genTemplate:Item Name=\"framework\"";
            string pt = $"genTemplate:Item Name=\"projectType\"";
            foreach (var line in lines)
            {
                if (!line.Contains(fx) && !line.Contains(pt))
                {
                    sb.AppendLine(line);
                }
            }

            File.Delete(manifest);
            File.WriteAllText(manifest, sb.ToString());
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForGenerationAsync))]
        [Trait("Type", "GenerationAllPagesAndFeatures")]
        public async Task GenAllPagesAndFeaturesAsync(string projectType, string framework, string platform, string language)
        {
            Func<ITemplateInfo, bool> selector =
                t => t.GetTemplateType() == TemplateType.Project
                    && t.GetProjectTypeList().Contains(projectType)
                    && t.GetFrameworkList().Contains(framework)
                    && t.GetPlatform() == platform
                    && !t.GetIsHidden()
                    && t.GetLanguage() == language;

            Func<ITemplateInfo, bool> templateSelector =
                t => (t.GetTemplateType() == TemplateType.Page || t.GetTemplateType() == TemplateType.Feature)
                    && t.GetFrameworkList().Contains(framework)
                    && t.GetPlatform() == platform
                    && !t.GetIsHidden();

            var projectName = $"{projectType}{framework}All{ShortLanguageName(language)}";

            await AssertGenerateProjectAsync(selector, projectName, projectType, framework, platform, language, templateSelector, BaseGenAndBuildFixture.GetDefaultName);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForGenerationAsync))]
        [Trait("Type", "GenerationRandomNames")]
        public async Task GenAllPagesAndFeaturesRandomNamesAsync(string projectType, string framework, string platform, string language)
        {
            Func<ITemplateInfo, bool> selector =
                t => t.GetTemplateType() == TemplateType.Project
                    && t.GetProjectTypeList().Contains(projectType)
                    && t.GetFrameworkList().Contains(framework)
                    && t.GetPlatform() == platform
                    && !t.GetIsHidden()
                    && t.GetLanguage() == language;

            Func<ITemplateInfo, bool> templateSelector =
                t => (t.GetTemplateType() == TemplateType.Page || t.GetTemplateType() == TemplateType.Feature)
                    && t.GetFrameworkList().Contains(framework)
                    && t.GetPlatform() == platform
                    && !t.GetIsHidden();

            var projectName = $"{projectType}{framework}AllRandom";

            await AssertGenerateProjectAsync(selector, projectName, projectType, framework, platform, language, templateSelector, BaseGenAndBuildFixture.GetRandomName);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForGenerationAsync))]
        [Trait("Type", "GenerationRightClick")]
        public async Task GenEmptyProjectWithAllRightClickItemsAsync(string projectType, string framework, string platform, string language)
        {
            var projectName = $"{projectType}{framework}AllRightClick";
            var projectPath = Path.Combine(_fixture.TestProjectsPath, projectName, projectName);

            await AssertGenerateRightClickAsync(projectName, projectType, framework, platform, language, true);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForGenerationAsync))]
        [Trait("Type", "GenerationRightClick")]
        public async Task GenCompleteProjectWithAllRightClickItemsAsync(string projectType, string framework, string platform, string language)
        {
            var projectName = $"{projectType}{framework}AllRightClick2";
            var projectPath = Path.Combine(_fixture.TestProjectsPath, projectName, projectName);

            await AssertGenerateRightClickAsync(projectName, projectType, framework, platform, language, false);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetPageAndFeatureTemplatesForGeneration), "MVVMLight")]
        [Trait("Type", "GenerationOneByOneMVVMLight")]
        public async Task GenMVVMLightOneByOneItemsAsync(string itemName, string projectType, string framework, string platform, string itemId, string language)
        {
            await AssertGenerationOneByOneAsync(itemName, projectType, framework, platform, itemId, language);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetPageAndFeatureTemplatesForGeneration), "CaliburnMicro")]
        [Trait("Type", "GenerationOneByOneCaliburnMicro")]
        public async Task GenCaliburnMicroOneByOneItemsAsync(string itemName, string projectType, string framework, string platform, string itemId, string language)
        {
            await AssertGenerationOneByOneAsync(itemName, projectType, framework, platform, itemId, language);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetPageAndFeatureTemplatesForGeneration), "Prism")]
        [Trait("Type", "GenerationOneByOnePrism")]
        public async Task GenPrismOneByOneItemsAsync(string itemName, string projectType, string framework, string platform, string itemId, string language)
        {
            await AssertGenerationOneByOneAsync(itemName, projectType, framework, platform, itemId, language);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetPageAndFeatureTemplatesForGeneration), "MVVMBasic")]
        [Trait("Type", "GenerationOneByOneMVVMBasic")]
        public async Task GenMVVMBasicOneByOneItemsAsync(string itemName, string projectType, string framework, string platform, string itemId, string language)
        {
            await AssertGenerationOneByOneAsync(itemName, projectType, framework, platform, itemId, language);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetPageAndFeatureTemplatesForGeneration), "CodeBehind")]
        [Trait("Type", "GenerationOneByOneCodeBehind")]
        public async Task GenCodeBehindOneByOneItemsAsync(string itemName, string projectType, string framework, string platform, string itemId, string language)
        {
            await AssertGenerationOneByOneAsync(itemName, projectType, framework, platform, itemId, language);
        }
    }
}
