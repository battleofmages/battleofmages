using UnityEngine;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	private Entity _target = null;
	private ushort _targetId = IDCreator.UndefinedId;

#region Properties
	// Target
	public Entity target {
		get {
			if(_targetId != IDCreator.UndefinedId) {
				if(_target == null || _target.id != _targetId) {
					Entity newTarget;
					
					if(Entity.idToEntity.TryGetValue(_targetId, out newTarget))
						target = newTarget;
				}
			}
			
			return _target;
		}
		
		set {
			// We need to use object.Equals because the comparison operator
			// doesn't work correctly with destroyed objects.
			if(object.Equals(_target, value))
				return;
			
			if(!uLink.Network.isServer && networkViewIsMine) {
				if(crossHair != null)
					crossHair.enabled = (value == null); //&& ToggleMouseLook.instance.mouseLook.enabled);
				
				if(_target != value && value != null && value.audio != null)
					value.audio.PlayOneShot(Sounds.toggleTargetFocus);
			}
			
			_target = value;
			
			if(_target == null)
				_targetId = IDCreator.UndefinedId;
			else
				_targetId = _target.id;
		}
	}

	// TargetId
	public ushort targetId {
		get {
			return _targetId;
		}
		
		set {
			_targetId = value;
			
			if(_targetId == IDCreator.UndefinedId) {
				target = null;
			} else {
				Entity newTarget;
				
				if(Entity.idToEntity.TryGetValue(_targetId, out newTarget))
					target = newTarget;
			}
		}
	}
	
	// Action target (where you can press the F key)
	public ActionTarget actionTarget {get; set;}
	
	// About to target
	public Entity aboutToTarget {get; set;}
	
	// Has target
	public bool hasTarget {
		get {
			return _target != null;
		}
	}
#endregion

#region Virtual
	// OnTargetReceived
	protected virtual void OnTargetReceived() {}
#endregion

#region RPCs
	[RPC]
	protected void SetTarget(ushort entityId) {
		targetId = entityId;
		OnTargetReceived();
	}
#endregion
}