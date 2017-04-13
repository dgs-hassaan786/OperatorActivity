using System;
using System.Collections.Generic;

namespace Operational.Model
{
    public class OperatorReport
    {
        public OperatorReport()
        {
            TotalChatLength = "0";
        }


        private double _totalChatLengthTime = 0;


        public int ID { get; set; }
        public string Name { get; set; }
        public int ProactiveSent { get; set; }
        public int ProactiveAnswered { get; set; }
        public int ProactiveResponseRate { get; set; }
        public int ReactiveReceived { get; set; }
        public int ReactiveAnswered { get; set; }
        public int ReactiveResponseRate { get; set; }
        public string TotalChatLength { get; set; }
        public string AverageChatLength { get; set; }
        public double TotalChatLengthTime
        {
            get
            {
                return _totalChatLengthTime;
            }

            set
            {
                if (value != 0)
                {
                    var t = TimeSpan.FromMinutes(value);
                    TotalChatLength = string.Format("{0:%d}d {0:hh}h {0:mm}m", t);
                }
                _totalChatLengthTime = value;
            }
        }
    }

    public class OperatorReportItems
    {
        public IEnumerable<OperatorReport> OperatorProductivity { get; set; }
    }
}
