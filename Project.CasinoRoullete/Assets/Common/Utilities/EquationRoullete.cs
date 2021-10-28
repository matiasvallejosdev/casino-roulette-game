
public static class EquationRoullete
{
    public static int EquationPayment(int quantityNumbers, int valueFicha)
    {
        int payment = 0;

        // Calculate the multiply depending the numbers abarcated of the  ficha
        int n = quantityNumbers;
        int multiply = (36 - n) / n;

        // Payment
        payment = valueFicha * multiply;

        return payment;
    }            
}