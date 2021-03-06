﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Templates.Core.Gen;
using Microsoft.Templates.Core.PostActions.Catalog;
using Microsoft.Templates.Core.PostActions.Catalog.Merge;
using Microsoft.Templates.Fakes;
using Xunit;

namespace Microsoft.Templates.Core.Test.PostActions.Catalog
{
    [Trait("ExecutionSet", "Minimum")]
    public class AddProjectReferenceToContextPostActionTest
    {
        [Fact]
        public async Task AddProjectReferenceToContext_ExecuteAsync()
        {
            var templateName = "Test";
            var projectName = "MyProject";
            var destPath = Path.GetFullPath(@".\DestinationPath\Project");
            var projectToAdd = @".\TestProject.csproj";

            GenContext.Current = new FakeContextProvider()
            {
                DestinationPath = destPath,
                GenerationOutputPath = destPath,
            };

            List<FakeCreationPath> testPrimaryOutputs = new List<FakeCreationPath>();
            testPrimaryOutputs.Add(new FakeCreationPath() { Path = projectToAdd });

            var postAction = new FakeTemplateDefinedPostAction(
                new Guid(AddProjectReferencesToContextPostAction.Id),
                new Dictionary<string, string>()
                {
                    { "fileIndex", "0" },
                    { "projectPath", projectName },
                });

            var mergePostAction = new AddProjectReferencesToContextPostAction(templateName, postAction, testPrimaryOutputs, new Dictionary<string, string>(), destPath);
            await mergePostAction.ExecuteAsync();

            Assert.True(GenContext.Current.ProjectReferences.ContainsKey(Path.Combine(destPath, projectName)));
            Assert.True(GenContext.Current.ProjectReferences[Path.Combine(destPath, projectName)].Contains(Path.GetFullPath(Path.Combine(destPath, projectToAdd))));
        }

        [Fact]
        public async Task AddSdkReferenceToContext_Execute_AlreadyThereAsync()
        {
            var templateName = "Test";
            var projectName = "MyProject";
            var destPath = Path.GetFullPath(@".\DestinationPath\Project");
            var projectToAdd = @".\TestProject.csproj";

            GenContext.Current = new FakeContextProvider()
            {
                DestinationPath = destPath,
                GenerationOutputPath = destPath,
            };

            GenContext.Current.ProjectReferences.Add(
                Path.Combine(destPath, projectName),
                new List<string>() { "FirstReference" });

            List<FakeCreationPath> testPrimaryOutputs = new List<FakeCreationPath>();
            testPrimaryOutputs.Add(new FakeCreationPath() { Path = projectToAdd });

            var postAction = new FakeTemplateDefinedPostAction(
                new Guid(AddProjectReferencesToContextPostAction.Id),
                new Dictionary<string, string>()
                {
                    { "fileIndex", "0" },
                    { "projectPath", projectName },
                });

            var mergePostAction = new AddProjectReferencesToContextPostAction(templateName, postAction, testPrimaryOutputs, new Dictionary<string, string>(), destPath);
            await mergePostAction.ExecuteAsync();

            Assert.True(GenContext.Current.ProjectReferences.ContainsKey(Path.Combine(destPath, projectName)));
            Assert.True(GenContext.Current.ProjectReferences[Path.Combine(destPath, projectName)].Contains(Path.GetFullPath(Path.Combine(destPath, projectToAdd))));
        }
    }
}
