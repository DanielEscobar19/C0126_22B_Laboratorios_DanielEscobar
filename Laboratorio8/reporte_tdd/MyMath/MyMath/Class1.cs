namespace MyMath
{
    public class Class1
    {

    }

    public class Rooter
    {
        public Rooter()
        {
        }

        public double SquareRoot(double input)
        {
            if (input <= 0.0)
            {
                throw new ArgumentOutOfRangeException();
            }
            double result = input;
            double previousResult = -input;
            while (Math.Abs(previousResult - result) > result / 1000)
            {
                previousResult = result;
                result = result - (result * result - input) / (2 * result);
            }
            return result;
        }
    }
} // Daniel Escobar Giraldo | C02748