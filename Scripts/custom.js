// Initialize wavesurfer
var wavesurfer = WaveSurfer.create({
    container: '#waveform',
    waveColor: 'violet',
    progressColor: 'purple'
});

// Load audio file
wavesurfer.load('@Model.FileUrl');
