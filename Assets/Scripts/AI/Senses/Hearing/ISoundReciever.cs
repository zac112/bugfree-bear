using UnityEngine;

public interface ISoundReciever
{
	void Recieve(ISoundEmitter emitter, GameObject sender); // not 100% sure about the args yet... so far they're good
}