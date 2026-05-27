using ActFlow.Archiver.Models;
using ActFlow.Models.Workflows;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActFlow.Archiver
{
	public interface IWorkflowArchive
	{
		/// <summary>
		/// Amount of completed workflows stored
		/// </summary>
		public int CompletedWorkflows { get; }
		/// <summary>
		/// The path to where to save completed workflow runs
		/// </summary>
		public string CompletedDirectory { get; set; }

		public Task Initialize();

		public List<ListWorkflowState> GetAllCompletedWorkflows();
		public CompletedWorkflowState GetCompletedWorkflow(Guid id);
		public void RemoveCompletedWorkflow(Guid id);
	}
}
