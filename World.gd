extends Node3D

@export var cameras : Array

@export var camera : int

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass



func _unhandled_input(event):
	
	if not event is InputEventKey:
		return
	
	if event.is_pressed() and event.keycode == KEY_V:
		camera = (camera + 1) % len(cameras)
		
		get_node(cameras[camera]).make_current()
