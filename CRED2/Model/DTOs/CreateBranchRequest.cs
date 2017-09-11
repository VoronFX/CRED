using CRED2.Services;

namespace CRED2.Model.DTOs
{
    public sealed class CreateBranchRequest
    {
        //[Required]
        //public Branch Branch { get; set; }        
        public string NN { get; set; }
    }

    public sealed class CreateBranchRunner : ITaskRequestRunner<CreateBranchRequest>
    {
        public void Run(TaskRequest taskRequest, CreateBranchRequest requestData)
        {
            throw new System.NotImplementedException();
        }
    }
}