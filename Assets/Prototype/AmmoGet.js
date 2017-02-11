#pragma strict

public var animationTime : float;
public var yPosDelta : float;
public var animationCurve : AnimationCurve;
public var alphaCurve : AnimationCurve;
public var scaleMultiplier : float;
public var deltaMultiplier : float;

private var animating : boolean;
private var startScale : float;
private var startPos : float;
private var rend : MeshRenderer;
private var initialized : boolean;


function Awake(){
	rend = GetComponent.<MeshRenderer>();
}
function Start(){
	Animate();
}

function Animate(){
	startPos = transform.parent.position.y;
	if(!animating){
		var t : float;
		animating = true;
		var frame : int = 1;
		var moveTo : float = (startPos+yPosDelta);
		// /transform.localScale = Vector3.one * scaleMultiplier;
		while(t<1){
			t+=Time.deltaTime/animationTime;
			t=Mathf.Clamp01(t);
			transform.position.y = Mathf.Lerp(startPos, moveTo, animationCurve.Evaluate(t));
			transform.localScale.y = Mathf.Lerp(1f, 0f, animationCurve.Evaluate(t));
			yield;
		}
		transform.position.y = startPos+yPosDelta;
		transform.localScale.y = 0f;
		animating = false;
	}
	//Destroy(gameObject);
}