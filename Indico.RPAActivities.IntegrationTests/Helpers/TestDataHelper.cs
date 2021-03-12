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

        public IEnumerable<string> GetWorkflowSubmissionFilePaths()
        {
            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            yield return Path.Combine(root, "./Helpers/Files/workflow-sample.pdf");
        }

        public IEnumerable<string> GetWorkflowSubmissionUris()
        {
            return new List<string> 
            { 
                @"https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
                @"https://www.wmaccess.com/downloads/sample-invoice.pdf"
            };
        }

        
    }
}
