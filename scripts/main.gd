extends Node2D

@onready var char1 = $players/CharacterBody2D

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	# Check if the action is being pressed in every frame
	if Input.is_action_pressed("test1"):
		print("Action 'test1' pressed")  # Debugging line
		char1.position = Vector2(100, -23)  # Set the character's position to (100, -23)
