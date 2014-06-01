using System.Collections.Generic;

[System.Serializable]
public class PaymentsList : JsonSerializable<PaymentsList> {
	public double balance;
	public double total;
	public List<string> payments;
}
