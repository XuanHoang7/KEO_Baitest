namespace KEO_Baitest.Data.DTOs
{
    public class ResponseGetDTO<T>
    {
        public int TotalRow { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public List<T> Datalist { get; set; } = new List<T>();
    }
}
