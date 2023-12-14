/*
 * Plans :-
 * Self Driving to destination
 * Agents will start at a parking spot and be in "Exit Parking Zone" mode.
 * Agents will be able to determine the most optimal path to go from one node to another
 * Agents will be able to exit from parking spots safely and reach its target. Once reached it will be able to park
 * Agent will need to be in its lane as well as avoid obstacle and reach parking spot. Once reached it will switch into parking mode
 * 
 * Parking
 * Agents will be able to park in a parking spot. Parking spots will be assigned and the Agent will learn to move to it
 * It will have to avoid obstacles like other cars, etc
 * Leaving Parking Zone
 * Agents will also learn to leave a parking zone safely without colliding
 * 
 * TRAININGS
 * 1. First we train parking alone
 * 2. Then we train exiting parking alone
 * 3. Then we combine both in one scenario
 * 4. Train Driving alone
 * 5. Train parking, exiting and driving all in one
 * 
 * PARKING SYSTEM :-
 * Rewards and penalties
 * We can reward the Agent for getting close to its parking spot and penalise if it's distance is more than the last time
 * If inside a parking spot we give the max reward of "Getting close to parking spot" as well as rewards for having many section of it's part inside the parking spot
 * We can heavily penalize for hitting other cars and end episode
 * We can reward heavily for being fully inside the parking zone and end episode.
 * 
 * TESTINGS:-
 * 1. Parking lot with 4 slots and 3 will be filled with cars, one will be target randomly assigned. Agent will spawn random location too along a road
 */