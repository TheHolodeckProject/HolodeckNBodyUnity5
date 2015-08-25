class gallery_item
{
	var name : String = "";
	var model : Transform;
	var pointer : Transform;
}

var rotate : boolean = true;

var gallery_items : gallery_item[];
private var timer : float = 1; // max = 1
private var current_item : int = 0;
private var go_to_item : int = 0;
private var old_go_to_item : int = 0;

var distance : float = 2;
var font : Font;
function update_position()
{
current_item = Mathf.Clamp(current_item,0,gallery_items.length-1);
	if(current_item!=go_to_item && timer==1)
	{
		old_go_to_item = go_to_item;
		timer=0;
		if(go_to_item<current_item)
		{
			go_to_item++;
		}
		if(go_to_item>current_item)
		{
			go_to_item--;
		}
	}
	timer=Mathf.Min(timer+(Time.deltaTime*3),1);
	for(var i = 0; i < gallery_items.length; i++)
	{
		gallery_items[i].pointer.position=Vector3(Mathf.Lerp(((i-old_go_to_item) * distance),((i-go_to_item) * distance),timer), 0,0);
		if(rotate)gallery_items[i].pointer.eulerAngles.y=Time.time*26;
		else gallery_items[i].pointer.eulerAngles.y=90;
	}
}

function Start()
{
	for(var i = 0; i < gallery_items.length; i++)
	{
		gallery_items[i].pointer = Instantiate(gallery_items[i].model, Vector3(i * distance, 0, 0), Quaternion.identity);
	}
}


function Update () {
update_position();
}

function OnGUI()
{
	GUI.skin.font=null;
	GUI.color=Color.white;
	if(GUI.Button(Rect(10,Screen.height-30,100,30),"Items"))
	{
		Application.LoadLevel("gallery");
	}
	if(GUI.Button(Rect(10+110,Screen.height-30,100,30),"Animations"))
	{
		Application.LoadLevel("gallery 2");
	}
	if(GUI.Button(Rect(10+220,Screen.height-30,100,30),"Shells"))
	{
		Application.LoadLevel("gallery 3"); 
	}
	if(GUI.Button(Rect(100,0,100,50),"Next"))
	{
		current_item++;
	}
	if(GUI.Button(Rect(0,0,100,50),"Previous"))
	{
		current_item--;
	}
	GUI.skin.font=font;
	GUI.color=Color.black;
	GUI.Label(Rect(Screen.width/2-205,105,400,100),gallery_items[go_to_item].name);
	GUI.Label(Rect(Screen.width/2-205,95,400,100),gallery_items[go_to_item].name);
	GUI.Label(Rect(Screen.width/2-195,95,400,100),gallery_items[go_to_item].name);
	GUI.Label(Rect(Screen.width/2-205,100,400,100),gallery_items[go_to_item].name);
	GUI.Label(Rect(Screen.width/2-195,100,400,100),gallery_items[go_to_item].name);
	GUI.Label(Rect(Screen.width/2-200,95,400,100),gallery_items[go_to_item].name);
	GUI.Label(Rect(Screen.width/2-200,105,400,100),gallery_items[go_to_item].name);
	GUI.Label(Rect(Screen.width/2-195,105,400,100),gallery_items[go_to_item].name);
	
	GUI.color=Color.white;
	GUI.Label(Rect(Screen.width/2-200,100,400,100),gallery_items[go_to_item].name);
	
}