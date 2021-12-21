namespace BoM.Players {
	public class Team {
		public int id;
		public int layer {
			get {
				return 8 + id;
			}
		}
	}
}
