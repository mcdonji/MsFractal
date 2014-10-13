namespace Fractal.Domain
{
    public class AndClause
    {
        public string Column { get; set; }
        public string Value { get; set; }

        public AndClause(string column, string value)
        {
            Value = value;
            Column = column;
        }
    }
}