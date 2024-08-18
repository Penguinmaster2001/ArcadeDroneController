extends Camera3D


@export var drone : Node3D

@export var track_speed : float


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	
	var lerp_direction = self.transform.looking_at(drone.position)
	
	self.transform = self.transform.interpolate_with(lerp_direction, track_speed)
