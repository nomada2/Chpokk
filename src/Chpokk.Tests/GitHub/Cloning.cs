﻿using System;
using System.Collections.Generic;
using System.IO;
using Arractas;
using Chpokk.Tests.GitHub.Infrastructure;
using Chpokk.Tests.Infrastructure;
using ChpokkWeb;
using ChpokkWeb.App_Start;
using ChpokkWeb.Remotes;
using ChpokkWeb.Repa;
using FubuMVC.Core;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Urls;
using FubuMVC.StructureMap;
using Ivonna.Framework;
using LibGit2Sharp;
using LibGit2Sharp.Tests.TestHelpers;
using MbUnit.Framework;
using System.Linq;
using StructureMap;
using Ivonna.Framework.Generic;

namespace Chpokk.Tests.GitHub {
	[TestFixture]
	public class Cloning: BaseCommandTest<CloneContext> {
		public override void Act() {
			const string repoUrl = "git://github.com/uluhonolulu/Chpokk-Scratchpad.git";
			var model = new CloneInputModel {PhysicalApplicationPath = Path.GetFullPath(".."), RepoUrl = repoUrl};
			Context.Container.Get<CloneController>().CloneRepo(model);
		}

		[Test]
		public void CreatesThePhysicalFiles() {
			var expectedFile = Path.Combine(Context.RepositoryPath, Context.FileName);
			var existingFiles = Directory.GetFiles(Context.RepositoryPath); 
			Assert.AreElementsEqual(new [] {expectedFile}, existingFiles);
		}

	}

	//[RunOnWeb]
	public class CloneContext : SimpleConfiguredContext, IDisposable {
		public string RepositoryPath { get; private set; }
		public string FileName { get; private set; }
		[SetUp]
		public override void Create() {
			base.Create();

			RepositoryPath  = Path.Combine(Path.GetFullPath(@".."), RepositoryInfo.Path);
			FileName = Guid.NewGuid().ToString();
			var content = "stuff";
			Api.CommitFile(FileName, content);
		}

		public void Dispose() {
			if (Directory.Exists(RepositoryPath))
				DirectoryHelper.DeleteDirectory(RepositoryPath);			
		}
	}

	public class StructureMapWorks {
		[Test]
		public void CanGetTheDefaultInstance() {
			var container = new Container();
			var expr = FubuApplication.For<ConfigureFubuMVC>()
				.StructureMap(container)
				.Bootstrap()
				;
			expr.Facility.Inject(typeof(IUrlRegistry), typeof(UrlRegistry));
			var registry = expr.Facility.Get<IUrlRegistry>();
		}
	}

	//public static class CheckoutExtensions {
	//    /// <summary>
	//    /// Performs a checkout of an existing branch.
	//    /// </summary>
	//    /// <param name="branch">The <see cref = "LibGit2Sharp.Branch" /> to be checked out.</param>
	//    /// <remarks>Overwrites the existing files.</remarks>
	//    public static void Checkout(this Repository repo, Branch branch) {
	//        repo.Refs.UpdateTarget("HEAD", branch.CanonicalName);
	//        repo.Reset(ResetOptions.Mixed, branch.CanonicalName);

	//        repo.WriteFilesToDisk(branch);
	//    }

	//    public static void WriteFilesToDisk(this Repository repo, Branch branch) {
	//        IEnumerable<string> fileNames = repo.Index.Select(entry => entry.Path).Where(path => branch.Tip[path] != null);
	//        string workingDirectory = repo.Info.WorkingDirectory;
	//        foreach (string fileName in fileNames) {
	//            byte[] content = ((Blob)(branch.Tip.Tree[fileName].Target)).Content;
	//            string filePath = Path.Combine(workingDirectory, fileName);
	//            File.WriteAllBytes(filePath, content);
	//        }
	//    }		
	//}


}