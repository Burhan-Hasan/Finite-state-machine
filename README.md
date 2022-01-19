# Finite-state-machine
Нужно реализовать простейший конечный автомат для TCP-сессий. 

Имеем следующий набор событий:
APP_PASSIVE_OPEN, APP_ACTIVE_OPEN, APP_SEND, APP_CLOSE, APP_TIMEOUT, RCV_SYN, RCV_ACK, RCV_SYN_ACK, RCV_FIN, RCV_FIN_ACK

Набор состояний:
CLOSED, LISTEN, SYN_SENT, SYN_RCVD, ESTABLISHED, CLOSE_WAIT, LAST_ACK, FIN_WAIT_1, FIN_WAIT_2, CLOSING, TIME_WAIT

Таблица переходов
CLOSED: APP_PASSIVE_OPEN -> LISTEN
CLOSED: APP_ACTIVE_OPEN  -> SYN_SENT
LISTEN: RCV_SYN          -> SYN_RCVD
LISTEN: APP_SEND         -> SYN_SENT
LISTEN: APP_CLOSE        -> CLOSED
SYN_RCVD: APP_CLOSE      -> FIN_WAIT_1
SYN_RCVD: RCV_ACK        -> ESTABLISHED
SYN_SENT: RCV_SYN        -> SYN_RCVD
SYN_SENT: RCV_SYN_ACK    -> ESTABLISHED
SYN_SENT: APP_CLOSE      -> CLOSED
ESTABLISHED: APP_CLOSE   -> FIN_WAIT_1
ESTABLISHED: RCV_FIN     -> CLOSE_WAIT
FIN_WAIT_1: RCV_FIN      -> CLOSING
FIN_WAIT_1: RCV_FIN_ACK  -> TIME_WAIT
FIN_WAIT_1: RCV_ACK      -> FIN_WAIT_2
CLOSING: RCV_ACK         -> TIME_WAIT
FIN_WAIT_2: RCV_FIN      -> TIME_WAIT
TIME_WAIT: APP_TIMEOUT   -> CLOSED
CLOSE_WAIT: APP_CLOSE    -> LAST_ACK
LAST_ACK: RCV_ACK        -> CLOSED

Реализация должна быть в виде консольного приложения, созданного на языке F#. На вход принимается набор событий в виде массива, на выходе новое состояние. 
Если переход невозможен, то возвращаем ERROR. Способ ввод/вывода на усмотрение разработчика.
Пример:
["APP_PASSIVE_OPEN", "APP_SEND", "RCV_SYN_ACK"] =>  "ESTABLISHED"
["APP_ACTIVE_OPEN"] =>  "SYN_SENT"
["APP_ACTIVE_OPEN", "RCV_SYN_ACK", "APP_CLOSE", "RCV_FIN_ACK", "RCV_ACK"] =>  "ERROR"