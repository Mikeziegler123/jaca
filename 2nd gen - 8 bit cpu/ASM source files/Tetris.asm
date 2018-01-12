// Tetris for JACA-2 homebrew CPU
// v.0.9.1

//#include	C:\Users\xdad\Source\Repos\jaca\2nd gen - 8 bit cpu\ASM source files\Sub_Multiply.asm

// memory mapping:
// start			end
// 0x000 (0)		0x2e8(so far...)		program code
// 0xb00 (2816)		0xb03					current piece pattern
// 0xb10 (2832)		0xb17					screen pattern
// 0xb20 (2848)		0xbc8					piece pattern data
// 0xc00 (3072)								variables (standard of the system)
// 0xd00 (3328)								include multiply




#defbyte	current_piece_num		// 0 to 6 (7 pieces available)
#defbyte	current_piece_position	// 0 to 3 (4 positions)
#defbyte	current_piece_x			// 0 to 7
#defbyte	current_piece_y			// 0 to 7
#defbyte	current_piece_width
#defbyte	current_piece_height



// start 0xb00, end 0xb03
// 4 bytes
//#defmem	current_piece_pattern

// start 0xb10, end 0xb17
// 8 bytes
//#defmem	screen_pattern



// ------------- Pieces data -------------

// initial x coord always = 2; y always 0
// using 24 bytes / piece; 7x24 = 168 bytes total

// piece #0:

#defmem	0x0b20		0b00011000	// piece pattern (position 0)
#defmem				0b00110000
#defmem				0b00000000
#defmem				0b00000000
#defmem				3			// width
#defmem				2			// height
//TODO:
//#defmem			0x27		// piece next position address (low byte); zero means piece cannot rotate

#defmem				0b00100000	// piece pattern (position 1)
#defmem				0b00110000
#defmem				0b00010000
#defmem				0b00000000
#defmem				2			// width
#defmem				3			// height
//TODO:
//#defmem			0x20		// piece next position address (low byte); zero means piece cannot rotate

#defmem				0b00011000	// piece pattern (position 2)
#defmem				0b00110000
#defmem				0b00000000
#defmem				0b00000000
#defmem				3			// width
#defmem				2			// height

#defmem				0b00100000	// piece pattern (position 3)
#defmem				0b00110000
#defmem				0b00010000
#defmem				0b00000000
#defmem				2			// width
#defmem				3			// height


// piece #1:

#defmem				0b00110000	// piece pattern (position 0)
#defmem				0b00011000
#defmem				0b00000000
#defmem				0b00000000
#defmem				3			// width
#defmem				2			// height

#defmem				0b00010000	// piece pattern (position 1)
#defmem				0b00110000
#defmem				0b00100000
#defmem				0b00000000
#defmem				2			// width
#defmem				3			// height

#defmem				0b00110000	// piece pattern (position 2)
#defmem				0b00011000
#defmem				0b00000000
#defmem				0b00000000
#defmem				3			// width
#defmem				2			// height

#defmem				0b00010000	// piece pattern (position 3)
#defmem				0b00110000
#defmem				0b00100000
#defmem				0b00000000
#defmem				2			// width
#defmem				3			// height


// piece #2:

#defmem				0b00001000	// piece pattern (position 0)
#defmem				0b00111000
#defmem				0b00000000
#defmem				0b00000000
#defmem				3			// width
#defmem				2			// height

#defmem				0b00110000	// piece pattern (position 1)
#defmem				0b00010000
#defmem				0b00010000
#defmem				0b00000000
#defmem				2			// width
#defmem				3			// height

#defmem				0b00111000	// piece pattern (position 2)
#defmem				0b00100000
#defmem				0b00000000
#defmem				0b00000000
#defmem				3			// width
#defmem				2			// height

#defmem				0b00100000	// piece pattern (position 3)
#defmem				0b00100000
#defmem				0b00110000
#defmem				0b00000000
#defmem				2			// width
#defmem				3			// height


// piece #3:

#defmem				0b00100000	// piece pattern (position 0)
#defmem				0b00111000
#defmem				0b00000000
#defmem				0b00000000
#defmem				3			// width
#defmem				2			// height

#defmem				0b00010000	// piece pattern (position 1)
#defmem				0b00010000
#defmem				0b00110000
#defmem				0b00000000
#defmem				2			// width
#defmem				3			// height

#defmem				0b00111000	// piece pattern (position 2)
#defmem				0b00001000
#defmem				0b00000000
#defmem				0b00000000
#defmem				3			// width
#defmem				2			// height

#defmem				0b00110000	// piece pattern (position 3)
#defmem				0b00100000
#defmem				0b00100000
#defmem				0b00000000
#defmem				2			// width
#defmem				3			// height


// piece #4:

#defmem				0b00010000	// piece pattern (position 0)
#defmem				0b00111000
#defmem				0b00000000
#defmem				0b00000000
#defmem				3			// width
#defmem				2			// height

#defmem				0b00010000	// piece pattern (position 1)
#defmem				0b00110000
#defmem				0b00010000
#defmem				0b00000000
#defmem				2			// width
#defmem				3			// height

#defmem				0b00111000	// piece pattern (position 2)
#defmem				0b00010000
#defmem				0b00000000
#defmem				0b00000000
#defmem				3			// width
#defmem				2			// height

#defmem				0b00100000	// piece pattern (position 3)
#defmem				0b00110000
#defmem				0b00100000
#defmem				0b00000000
#defmem				2			// width
#defmem				3			// height


// piece #5:

#defmem				0b00111100	// piece pattern (position 0)
#defmem				0b00000000
#defmem				0b00000000
#defmem				0b00000000
#defmem				4			// width
#defmem				1			// height

#defmem				0b00100000	// piece pattern (position 1)
#defmem				0b00100000
#defmem				0b00100000
#defmem				0b00100000
#defmem				1			// width
#defmem				4			// height

#defmem				0b00111100	// piece pattern (position 2)
#defmem				0b00000000
#defmem				0b00000000
#defmem				0b00000000
#defmem				4			// width
#defmem				1			// height

#defmem				0b00100000	// piece pattern (position 3)
#defmem				0b00100000
#defmem				0b00100000
#defmem				0b00100000
#defmem				1			// width
#defmem				4			// height


// piece #6:

#defmem				0b00110000	// piece pattern (position 0)
#defmem				0b00110000
#defmem				0b00000000
#defmem				0b00000000
#defmem				2			// width
#defmem				2			// height

#defmem				0b00110000	// piece pattern (position 1)
#defmem				0b00110000
#defmem				0b00000000
#defmem				0b00000000
#defmem				2			// width
#defmem				2			// height

#defmem				0b00110000	// piece pattern (position 2)
#defmem				0b00110000
#defmem				0b00000000
#defmem				0b00000000
#defmem				2			// width
#defmem				2			// height

#defmem				0b00110000	// piece pattern (position 3)
#defmem				0b00110000
#defmem				0b00000000
#defmem				0b00000000
#defmem				2			// width
#defmem				2			// height



#org	0x000

main_init:

	// pattern of clear screen
	LD A, 0
	ST [0xb10], A
	ST [0xb11], A
	ST [0xb12], A
	ST [0xb13], A
	ST [0xb14], A
	ST [0xb15], A
	ST [0xb16], A
	//LD A, 0b00001100
	ST [0xb17], A
	
	// initialize variables
	LD A, 3
	ST #current_piece_num, A
	LD A, 0
	ST #current_piece_position, A
	
	
next_piece:

	// current_piece_num++
	LD A, #current_piece_num
	INC A
	
	// if(current_piece_num == 7) current_piece_num = 0
	LD B, A
	LD C, 7		
	SUB B, C
	JP Z, :reset_current_piece_num
	JP :next_piece_1
	
reset_current_piece_num:
	LD A, 0

next_piece_1:

	ST #current_piece_num, A



	CALL :load_piece
	
	
	
	// Place piece in the starting position
	LD A, 2
	ST #current_piece_x, A

	LD A, 0
	ST #current_piece_y, A
	
	

// --- Draw screen
	LD D, 8
	LD C, 0
	LD H, 0x0b
	LD L, 0x10

loop_draw_screen:

	; LD A, [HL]
	OUT 1, A, C
	LD B, C
	INC B
	LD C, B
	SUB B, D
	JP Z, :end_draw_screen
	INC L
	JP :loop_draw_screen
	
end_draw_screen:

	// refresh screen
	LD C, 8	// constant to refresh screen
	OUT 1, A, C
	
	
	
main_loop:

	CALL :draw_piece

	IN 0, B
	LD E, B
	
	// check '6' key (move right)
	LD D, 0x36
	SUB B, D
	JP Z, :move_right

	LD B, E

	// check '4' key (move left)
	LD D, 0x34
	SUB B, D
	JP Z, :move_left

	LD B, E

	// check '8' key (rotate piece)
	LD D, 0x38
	SUB B, D
	JP Z, :rotate_piece

main_loop_1:

	JP :move_down
	
	JP :main_loop
	

	
move_down:

	LD A, #current_piece_y
	INC A
	LD C, #current_piece_height
	LD B, A
	ADD B, C
	
	// check if is the last line
	// if ((y+1) + height == 9) { // don't let move down } else y++
	LD C, 9
	SUB B, C
	
	//JP Z, :move_down_end
	JP Z, :place_piece
	
	LD B, A

	// check if there is a collision between piece and screen
	CALL :check_collision
	JP Z, :move_down_cont
	JP :place_piece

move_down_cont:
	
	ST #current_piece_y, B

	JP :main_loop
	
place_piece:

	LD A, #current_piece_y
	
	// if(y == 0) game_over
	DNW A
	JP Z, :game_over

	LD H, 0x0b
	LD E, 0x10
	ADD A, E
	LD E, A		// base addr for screen_pattern (screen_pattern + current_piece_y)
	
	LD F, 0x00	// base addr for piece_pattern

	// HE : addr screen pattern
	// HF : piece pattern

place_piece_loop:

	LD L, E
	LD A, [HL]		// screen pattern behind piece's line
	LD C, A

	LD L, F
	LD A, [HL] 		// piece line pattern

	OR A, C			// merge the two patterns

	LD L, E
	ST [HL], A		// save to the screen pattern

	// F++
	LD A, F
	INC A
	LD F, A
	// if(F == 4) next_piece
	LD A, 0x04
	SUB A, F
	JP Z, :next_piece
	
	// E++
	LD A, E
	INC A
	LD E, A
	JP :place_piece_loop


// TODO: remove (not being used anymore)
move_down_end:

	RET



// Load piece from memory, based in current_piece_num and current_piece_position variables
load_piece:

	// Calc address of piece pattern in memory:
	// =base addr (0xb20) + (current_piece_num * (4*6)) + (current_piece_position * 6)
	LD H, 0x0b

	LD A, #current_piece_num
	LD B, 24
	
	// It's not possible to have a subroutine (CALL) inside another
	//CALL :multiply
		LD D, A
		LD A, 0x0
		DNW B
	mu_loop:
		JP Z, :mu_end
		ADD A, D
		DEC B
		JP :mu_loop
	mu_end:
	
	LD C, A
    
	LD A, #current_piece_position
	LD B, 6
	
	//CALL :multiply
		LD D, A
		LD A, 0x0
		DNW B
	mu_loop_1:
		JP Z, :mu_end_1
		ADD A, D
		DEC B
		JP :mu_loop_1
	mu_end_1:

	ADD A, C
	LD C, 0x20
	ADD A, C
	LD L, A

	

	// Load current_piece_pattern:
	LD A, [HL]
	ST [0xb00], A
	
	INC L
	LD A, [HL]
	ST [0xb01], A
	
	INC L
	LD A, [HL]
	ST [0xb02], A
	
	INC L
	LD A, [HL]
	ST [0xb03], A

	
	INC L
	LD A, [HL]
	ST #current_piece_width, A
	
	INC L
	LD A, [HL]
	ST #current_piece_height, A

	RET


	
// check collision between piece and screen pattern
// returns in Z flag: 0 - no collision; 	1 - collision
// expect A with piece Y coord to be tested
// destroys A, H, L, C, E, F
check_collision:

	LD H, 0x0b	// base addr of screen pattern (hi byte)
	LD E, 0x10	// base addr of screen pattern (lo byte)
	ADD A, E
	LD E, A		// addr of screen_pattern to be tested (screen_pattern + current_piece_y)
	
	LD F, 0x00	// base addr of piece_pattern (low byte)

	// HE : addr screen pattern
	// HF : piece pattern

check_collision_loop:

	LD L, E
	LD A, [HL]		// screen pattern behind piece's line
	LD C, A

	LD L, F
	LD A, [HL] 		// piece line pattern

	AND A, C		// AND the two patterns, any number different from zero means collision
	JP Z, :check_collision_loop_cont
	JP :check_collision_end

check_collision_loop_cont:
	
	// F++
	LD A, F
	INC A
	LD F, A
	// if(F == 4) check_collision_end
	LD A, 0x04
	SUB A, F
	JP Z, :check_collision_end
	
	// E++
	LD A, E
	INC A
	LD E, A
	JP :check_collision_loop

check_collision_end:

	RET

	
	
move_right:

	LD A, #current_piece_x
	INC A
	LD C, #current_piece_width
	LD B, A
	ADD B, C
	
	// if ((x+1) + width == 9) { // don't let move right } else x++
	LD C, 9
	SUB B, C
	
	JP Z, :main_loop_1
	
	ST #current_piece_x, A

	CALL :shr_piece_pattern

	LD A, #current_piece_y
	CALL :check_collision
	JP Z, :main_loop_1

move_right_undo:
	
	LD A, #current_piece_x
	DEC A
	ST #current_piece_x, A

	CALL :shl_piece_pattern

	JP :main_loop_1



	
move_left:

	// if(x == 0) return
	LD A, #current_piece_x
	DNW A
	JP Z, :main_loop_1

	DEC A
	ST #current_piece_x, A

	CALL :shl_piece_pattern
			
	LD A, #current_piece_y
	CALL :check_collision
	JP Z, :main_loop_1

move_left_undo:
	
	LD A, #current_piece_x
	INC A
	ST #current_piece_x, A

	CALL :shr_piece_pattern

	JP :main_loop_1

	

	
rotate_piece:

	// current_piece_position++
	LD A, #current_piece_position
	INC A

	// if(current_piece_position == 4) current_piece_position == 0
	LD C, 0b00000011		// mask to get only the last two bits
	AND A, C

	ST #current_piece_position, A

	// TODO: save current piece pattern to be used in rotate_piece_undo

	CALL :load_piece

	// TODO: shift left or right the piece just loaded based on current x position
	
	// if(check_collision) rotate_piece_undo else return;
	LD A, #current_piece_y
	CALL :check_collision
	JP Z, :main_loop_1

rotate_piece_undo:
	
	// current_piece_position--
	LD A, #current_piece_position
	DEC A

	// if(current_piece_position == 0) current_piece_position == 3
	LD C, 0b00000011		// mask to get only the last two bits
	AND A, C

	ST #current_piece_position, A

	CALL :load_piece

	JP :main_loop_1

	
	
	
// Shift left piece_pattern	
shl_piece_pattern:

	LD H, 0x0b
	LD L, 0x00

	LD C, 0x04		// Constant to test end of loop

shl_piece_pattern_loop:

	LD A, [HL]
	SHL A
	ST [HL], A
	
	INC L
	LD A, L
	SUB A, C
	JP Z, :shl_piece_pattern_end
	
	JP :shl_piece_pattern_loop

shl_piece_pattern_end:
	RET
	


	
// Shift right piece_pattern	
shr_piece_pattern:

	LD H, 0x0b
	LD L, 0x00

	LD C, 0x04		// Constant to test end of loop

shr_piece_pattern_loop:

	LD A, [HL]
	SHR A
	ST [HL], A
	
	INC L
	LD A, L
	SUB A, C
	JP Z, :shr_piece_pattern_end
	
	JP :shr_piece_pattern_loop

shr_piece_pattern_end:
	RET
	


	
draw_piece:
	
	LD B, #current_piece_y
	LD C, B

	// --- redraw screen line above piece
	
	// if(y == 0) draw_piece_1 else D--
	DNW B
	JP Z, :draw_piece_1
	LD H, B
	DEC H
	LD D, H
	
	// get screen pattern of line above the piece
	LD H, 0x0b
	LD L, 0x10
	ADD L, D
	LD A, [HL]
	
	OUT 1, A, D
	
draw_piece_1:
	
	LD H, 0x0b	// hi byte of base addr of screen pattern lines
	LD L, 0x10	// lo byte

	ADD L, C	// sum screen pattern addr with piece Y

	
	// get screen pattern of line behind the first line of piece
	LD A, [HL]
	LD E, A

	// get pattern of first line of piece, make OR with screen line and draw to screen
	LD A, [0xb00]
	OR A, E
	OUT 1, A, C
	
	
	INC B
	LD C, B
	INC L
	
	
	// ------------------- (2)
	LD A, [HL]
	LD E, A

	// (2)
	LD A, [0xb01]
	OR A, E
	OUT 1, A, C
	
	
	
	INC B
	LD C, B
	INC L
	
	
	// --------------------- (3)
	LD A, [HL]
	LD E, A

	// (3)
	LD A, [0xb02]
	OR A, E
	OUT 1, A, C
	
	
	
	INC B
	LD C, B
	INC L
	
	
	// ------------------- (4)
	LD A, [HL]
	LD E, A

	// (4)
	LD A, [0xb03]
	OR A, E
	OUT 1, A, C
	
	
	
	
	




	//LD A, [0xb01]
	//OUT 1, A, C
	//
	//INC B
	//LD C, B
    //
	//LD A, [0xb02]
	//OUT 1, A, C
	//
	//INC B
	//LD C, B
    //
	//LD A, [0xb03]
	//OUT 1, A, C
	

	// refresh screen
	LD C, 8	// constant to refresh screen
	OUT 1, A, C

	RET

	
game_over:
	LD A, 0x47	// 'G'
	OUT 0, A
	LD A, 0x61	// 'a'
	OUT 0, A
	LD A, 0x6d	// 'm'
	OUT 0, A
	LD A, 0x65	// 'e'
	OUT 0, A
	LD A, 0x20	// ' '
	OUT 0, A
	LD A, 0x4f	// 'O'
	OUT 0, A
	LD A, 0x76	// 'v'
	OUT 0, A
	LD A, 0x65	// 'e'
	OUT 0, A
	LD A, 0x72	// 'r'
	OUT 0, A
game_over_1:
	JP :game_over_1

	
	

