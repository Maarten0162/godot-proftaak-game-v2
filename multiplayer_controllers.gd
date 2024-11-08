extends Node

var num_players = Input.get_connected_joypads().size() + 1
var players: Array = []

# Called when the node enters the scene tree for the first time.
func _ready():
	var new_connection: int
	new_connection = Input.joy_connection_changed.connect(gamepad_connection_changed)

func gamepad_connection_changed(device: int, connected: bool):
	if connected:
		num_players = Input.get_connected_joypads().size()
		print("connected device {d}.".format({"d":device}))
		add_player()
	else:
		pass

func add_player():
	var Player = load("res://scenes/main.tscn")
	var player = Player.instantiate()
	players.append(player)
	print(players)
