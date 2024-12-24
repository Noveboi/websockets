export default class MyWebSocket {
    constructor(serverPort, receive) {
        if (serverPort < 0 || serverPort > 65565)
            throw new RangeError();

        this.serverPort = serverPort;
        this.socket = new WebSocket(`ws://localhost:${serverPort}/ws`)
        
        this.socket.addEventListener('message', e => {
            receive(e.data)
        })
    
        this.socket.addEventListener('error', e => {
            console.log(e)
        })
    }

    send(message) {
        this.socket.send(message)
    }

    connect(username) {
        const initialPayload = JSON.stringify({
            username: username
        })

        console.log(initialPayload)
        this.socket.send(initialPayload)
    }
}