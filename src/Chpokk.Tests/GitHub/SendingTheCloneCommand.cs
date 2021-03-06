﻿using System;
using System.IO;
using System.Web.Hosting;
using Arractas;
using CThru;
using CThru.BuiltInAspects;
using Chpokk.Tests.Infrastructure;
using ChpokkWeb.Features.Exploring;
using ChpokkWeb.Features.Remotes;
using ChpokkWeb.Features.Remotes.Clone;
using FubuMVC.Core.Urls;
using Ivonna.Framework;
using Ivonna.Framework.Generic;
using LibGit2Sharp;
using MbUnit.Framework;
using System.Linq;

namespace Chpokk.Tests.GitHub {
	[TestFixture, RunOnWeb]
	public class SendingTheCloneCommand : BaseQueryTest<SimpleConfiguredContext, Spy<string>> {
		[Test]
		public void ShouldCallTheCloneCommandOnce() {
			Assert.AreEqual(1, Result.Results.Count());
		}

		[Test]
		public void ShouldCloneToThePathCorrespondingToTheRepoUrl() {
			var path = Result.Results.First();
			var manager = Context.Container.Get<RepositoryManager>();
			var expectedPath = Path.Combine("D:\\Projects\\Chpokk\\src\\ChpokkWeb", manager.GetPathFor("stub")) ;
			Assert.AreEqual(expectedPath, path, StringComparison.InvariantCultureIgnoreCase);
		}

		[FixtureSetUp]
		public override void Arrange() {
			base.Arrange();
		}

		//[TestedMethod]
		public override Spy<string> Act() {
			var spy = new Spy<string>(info => info.TypeName.EndsWith("CloneController") && info.MethodName == "CloneGitRepository", args => (string)args.ParameterValues[1]);
			CThruEngine.AddAspect(spy);
			CThruEngine.AddAspect(Stub.For<CloneController>("CloneGitRepository"));
			var url = Context.Container.Get<IUrlRegistry>().UrlFor<CloneInputModel>();
			new TestSession().Post(url, new CloneInputModel {RepoUrl = "stub"});
			return spy;
		}

	}

	//public class TestedMethodAttribute : Attribute {}
}
