using System.Collections.Generic;
using System.Linq;
using Indico.RPAActivities.Activities;

namespace Indico.RPAActivities.IntegrationTests.Helpers
{
    internal class TestDataHelper
    {
        public int GetDataSetId() => new ListDatasets().Invoke().First().Id;

        public int GetWorkflowId() => new ListWorkflows { DatasetID = GetDataSetId() }.Invoke().First().Id;

        public IEnumerable<string> GetWorkflowSubmissionFilePaths()
        {
            return new List<string> { "./Helpers/Files/workflow-sample.pdf" };
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
