import MyWebSocket from "./socket.js";

const serverPort = 5191
const socket = new MyWebSocket(serverPort, appendMessage)

const messageList = getRequiredElementById('message-list')
const chatForm = getRequiredElementById('chat-form')
const connectForm = getRequiredElementById('connect-form')
const connectArea = getRequiredElementById('connect-area')

connectForm.addEventListener('submit', e => {
    e.preventDefault()

    const formData = new FormData(connectForm)
    const username = formData.get('username')

    socket.connect(username)
    connectForm.remove()
    connectArea.appendChild(loggedInMessage(username))
})

chatForm.addEventListener('submit', e => {
    e.preventDefault()

    const formData = new FormData(chatForm)
    const message = formData.get('chat-text')

    socket.send(message)
})

function loggedInMessage(username) {
    const h2 = document.createElement('h2')
    const content = document.createTextNode(`Connected as '${username}'`)

    h2.appendChild(content)
    return h2
}

function appendMessage(message) {
    const newItem = document.createElement('li')
    const content = document.createTextNode(message)

    newItem.appendChild(content)

    messageList.appendChild(newItem)
}

function getRequiredElementById(id) {
    const element = document.getElementById(id)
    if (element == null) {
        throw new Error(`Element with ID '${id}' does not exist!`)
    }

    return element
}