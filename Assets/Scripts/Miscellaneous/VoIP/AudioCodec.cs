public delegate void SendFunc(byte[] bytes, int len);

public interface AudioCodec {
	void Encode(float[] samples, SendFunc send);
	float[] Decode(byte[] samples, int len);
}