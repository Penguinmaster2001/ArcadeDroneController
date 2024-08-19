extends Node3D

var cameras : Array

var camera : int

# Called when the node enters the scene tree for the first time.
func _ready():
	
	find_cameras(self)
	
	cameras[camera].make_current()
	
	


func find_cameras(node):
	if node is Camera3D:
		cameras.append(node)
	
	for child in node.get_children():
		find_cameras(child)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass



func _unhandled_input(event):
	
	if not event is InputEventKey:
		return
	
	if event.is_pressed() and event.keycode == KEY_V:
		camera = (camera + 1) % len(cameras)
		
		cameras[camera].make_current()
