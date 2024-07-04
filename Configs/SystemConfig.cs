namespace PRN_ProjectAPI.Configs
{
    public class SystemConfig
    {
        public string ContactUrl {  get; set; }
        public string ImageUrl { get; set; }
        public string UploadFilePath { get; set; }
        public SystemConfig()
        {
            ContactUrl = "http://localhost:5247/api/Contact/";
            ImageUrl = "http://localhost:5247/api/Image/";
            UploadFilePath = "E:\\Self-Project\\Upload_File\\user\\image";
        }
    }
}
