using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Indico.RPAActivities.Activities;

namespace Indico.RPAActivities.IntegrationTests.Helpers
{
    internal class TestDataHelper
    {
        public int GetDataSetId() => new ListDatasets().Invoke().First().Id;

        public int GetWorkflowId() => new ListWorkflows { DatasetID = GetDataSetId() }.Invoke().First().Id;

        public IEnumerable<string> GetWorkflowSubmissionFilePaths() => new[] {GetFilePath()};

        public string GetFilePath() => Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "./Helpers/Files/workflow-sample.pdf");

        public IEnumerable<string> GetWorkflowSubmissionUris()
        {
            return new List<string>
            {
                @"https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                @"https://www.wmaccess.com/downloads/sample-invoice.pdf"
            };
        }


        public int GetSubmissionId() =>
            new WorkflowSubmission
            {
                WorkflowID = GetWorkflowId(),
                FilePaths = new InArgument<List<string>>(_ => GetWorkflowSubmissionFilePaths().ToList()),
            }
                .Invoke()
                .Single();

        public int GetModelGroupId() => new ListDatasets().Invoke().First().ModelGroups.First().Id;
    }
}
