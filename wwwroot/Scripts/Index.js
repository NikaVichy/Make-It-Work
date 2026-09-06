// function parseCustomDate(str) {
//     const match = str.match(/^(\d{2})\.(\d{2})\.(\d{4})\s(\d{2}:\d{2}:\d{2})\s([+-]\d{2}:\d{2})$/);
//     if (!match) return null;
    
//     console.log(match)
//     const [, day, month, year, time, offset] = match;
//     return new Date(`${year}-${month}-${day}T${time}${offset}`);
// }

function FormatTimeFromDate(sTime){
    if(!sTime) return "";
    
    return new Date(sTime).toLocaleString("ru").replace(",", "").slice(0, 16)
}

// remove?
// function toDatetimeLocal(dateStr) {
//     const date = new Date(dateStr);
//     if (isNaN(date)) return '';
    
//     const pad = n => String(n).padStart(2, '0');
//     return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
// }

function autoResize(element) {
    if (!element) return;
    element.style.height = 0;
    element.style.height = `${element.scrollHeight}px`;
}

const ui = {
    wrapper: document.getElementById('task-panel-wrapper'),
    idField: document.getElementById('task-panel-id'),
    name: document.getElementById('task-panel-name'),
    description: document.getElementById('task-panel-description'),
    author: document.getElementById('task-author'),
    project: document.getElementById('project-id'),
    dueTo: document.getElementById('task-panel-dueTo'),
    creation: document.getElementById('task-panel-creation'),

    fp: flatpickr("#task-panel-dueTo", {
        enableTime: true,       
        time_24hr: true,        
        locale: "ru",           
        altInput: true,         
        altFormat: "d.m.Y H:i", 
        dateFormat: "Y-m-d H:i",
        allowInput: true, 
    }),
    
    btnCreate: document.getElementById('task-panel-submit-create'),
    btnUpdate: document.getElementById('task-panel-submit-update'),
    btnClose: document.getElementById('task-panel-close'),
    btnNewTask: document.getElementById('new-task-button'),
};

const show = (el, displayType = 'block') => { if (el) el.style.display = displayType; };
const hide = (el) => { if (el) el.style.display = 'none'; };

function resetTaskPanel() {
    ui.idField.value = 0;
    ui.name.value = '';
    ui.description.value = '';

    ui.author.textContent = '-';

    ui.project.innerHTML = '';
    ui.project.disabled = false;

    ui.fp.clear();

    ui.creation.textContent = '-';

    hide(ui.btnUpdate);
    show(ui.btnCreate);
    show(ui.wrapper, 'flex');

    autoResize(ui.name);
    autoResize(ui.description);
}

function fillTaskPanel(task) {
    ui.idField.value = task.id;
    ui.name.value = task.taskName;

    ui.author.textContent = `@${task.authorName}`;

    ui.project.replaceChildren();
    ui.project.add(new Option(task.projectName, task.projectId));
    ui.project.disabled = true;

    ui.description.value = task.taskDescription;
    ui.creation.textContent = " " + FormatTimeFromDate(task.createdAt)
    
    ui.fp.setDate(task.dueTo ? new Date(task.dueTo) : "?", false);

    show(ui.btnUpdate);
    hide(ui.btnCreate);
    show(ui.wrapper, 'flex');

    autoResize(ui.name);
    autoResize(ui.description);
}

document.querySelectorAll('.deadline').forEach(el => {
    var date = el.textContent;
    var parsedDate = FormatTimeFromDate(date);

    if (parsedDate < new Date()) {
        el.style.color = '#ff0000';
    }
    el.textContent = parsedDate;
});

document.querySelectorAll('.task-name').forEach(link => {
    link.addEventListener('click', async () => {
        const id = link.dataset.id;
        
        try {
            const response = await fetch(`?handler=TaskDetails&taskId=${id}`);
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }

            const task = await response.json();
            fillTaskPanel(task);
        } catch (error) {
            console.error('Unable to load task:', error);
        }
    });
});

document.querySelectorAll('.input-colorer').forEach(field => {
    field.addEventListener('input', () => autoResize(field));
});

ui.btnNewTask.addEventListener('click', async () => {
    try {
        const response = await fetch(`?handler=UserProjects`)
        if(!response.ok) {
            throw new Error(`Error: ${response.status}`);
        }

        const projects = await response.json();
        resetTaskPanel();

        projects.forEach(project => {
            option = new Option(project.name, project.id);
            ui.project.add(option)
        });
    } 
    catch (error) {
        console.error('Unable to load projects:', error)
    }

});
ui.btnClose.addEventListener('click', () => hide(ui.wrapper));