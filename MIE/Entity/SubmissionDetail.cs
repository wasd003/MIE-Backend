using System;
namespace MIE.Entity
{
    public class SubmissionDetail
    {
        public SubmissionDetail(int submissionCnt, int aCCnt, int wACnt, int tLECnt, int cECnt, int rECnt)
        {
            SubmissionCnt = submissionCnt;
            ACCnt = aCCnt;
            WACnt = wACnt;
            TLECnt = tLECnt;
            CECnt = cECnt;
            RECnt = rECnt;
        }

        public int SubmissionCnt { get; set; }

        public int ACCnt { get; set; }

        public int WACnt { get; set; }

        public int TLECnt { get; set; }

        public int CECnt { get; set; }

        public int RECnt { get; set; }
    }
}
