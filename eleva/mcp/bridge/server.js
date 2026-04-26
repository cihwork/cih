#!/usr/bin/env node
'use strict';

const http = require('http');
const readline = require('readline');

const ELEVA_URL = process.env.ELEVA_URL || 'http://localhost:17009';
const INSTANCE_ID = process.env.ELEVA_INSTANCE_ID || '1';
const parsed = new URL(ELEVA_URL);
const PORT = parseInt(parsed.port || '17009');
const HOST = parsed.hostname;

let pending = 0;
let inputClosed = false;

function post(path, body) {
  return new Promise((resolve, reject) => {
    const data = JSON.stringify(body);
    const opts = {
      hostname: HOST,
      port: PORT,
      path,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Content-Length': Buffer.byteLength(data),
        'X-Instance-Id': INSTANCE_ID,
      },
    };
    const req = http.request(opts, (res) => {
      let buf = '';
      res.on('data', (c) => (buf += c));
      res.on('end', () => {
        try { resolve(JSON.parse(buf)); }
        catch { resolve({ error: buf }); }
      });
    });
    req.on('error', reject);
    req.write(data);
    req.end();
  });
}

function send(obj) {
  process.stdout.write(JSON.stringify(obj) + '\n');
}

function maybeExit() {
  if (inputClosed && pending === 0) process.exit(0);
}

const rl = readline.createInterface({ input: process.stdin, terminal: false });

rl.on('line', async (line) => {
  line = line.trim();
  if (!line) return;

  let msg;
  try { msg = JSON.parse(line); }
  catch { return; }

  const { id, method, params } = msg;
  pending++;

  try {
    const result = await post('/mcp', { jsonrpc: '2.0', id, method, params });
    send(result);
  } catch (err) {
    send({ jsonrpc: '2.0', id: id ?? null, error: { code: -32603, message: err.message } });
  } finally {
    pending--;
    maybeExit();
  }
});

rl.on('close', () => {
  inputClosed = true;
  maybeExit();
});
