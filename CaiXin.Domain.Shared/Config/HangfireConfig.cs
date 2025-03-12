namespace CaiXin.Domain.Shared.Config
{
    public class HangfireConfig
    {
        /// <summary>
        /// 是否启用HangfireServer
        /// </summary>
        public bool ServerEnable { get; set; }

        /// <summary>
        /// 是否启用Hangfire仪表盘
        /// </summary>
        public bool DashboardIsEnable { get; set; }
    }
}
