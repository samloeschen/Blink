#pragma strict

public var fadeTime : float;
private var rend : MeshRenderer;

public var fadeIn : boolean;
public var fadeOut : boolean = true;
private var t : float;
function Start() {
	rend=GetComponent.<MeshRenderer>();
	var height : float = (Camera.main.orthographicSize*2f)+5f;
	var width : float = (height*Screen.width/Screen.height)+5f;
	transform.localScale= Vector3(width, height, 0f);
}

function Update () {
	var c : Color = Color.black;
	transform.position.x=Camera.main.transform.position.x;
	transform.position.y=Camera.main.transform.position.y;
	if(fadeIn){
		t+=Time.deltaTime;
		c.a=Mathf.Lerp(1,0,t/fadeTime);
		rend.material.SetColor("_Color",c);
	}
	if(fadeOut){
		t+=Time.deltaTime;
		c.a=Mathf.Lerp(0,1,t/fadeTime);
		rend.material.SetColor("_Color",c);

	}
	if(t>=fadeTime){
		t=0f;
		//this.enabled=false;
		fadeIn=false;
		fadeOut=false;
	}

}
function FadeIn(){
	fadeIn=true;
	fadeOut=false;
}
function FadeOut(){
	fadeOut=true;
	fadeIn=false;
}
