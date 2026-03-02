import sqlite3
import uuid
import json
import os
import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from datetime import datetime
from flask import Flask, request, jsonify, send_from_directory

app = Flask(__name__, static_folder='static')
DB_PATH = os.path.join(os.path.dirname(__file__), 'maturidade.db')

QUESTOES = [
    {
        "sessao": 1,
        "titulo": "Estrutura e Processos",
        "perguntas": [
            {
                "id": 1,
                "texto": "Os processos internos são documentados e seguidos por todas as áreas?",
                "opcoes": [
                    "Não há processos documentados. Cada área opera de forma independente.",
                    "Alguns processos estão mapeados, mas não são seguidos de forma consistente.",
                    "A maioria dos processos estão documentados, mas há dificuldades na implementação.",
                    "Processos bem definidos e aplicados regularmente.",
                    "Todos os processos são documentados, revisados periodicamente e otimizados continuamente."
                ]
            },
            {
                "id": 2,
                "texto": "Há um fluxo claro para resolução de problemas operacionais?",
                "opcoes": [
                    "Problemas são resolvidos de forma improvisada e sem padrão.",
                    "Algumas diretrizes existem, mas não são seguidas de forma consistente.",
                    "Há um processo definido, mas nem sempre é eficiente.",
                    "O fluxo de resolução é bem estruturado e aplicado pela equipe.",
                    "A empresa tem um sistema ágil para detectar, corrigir e prevenir falhas operacionais."
                ]
            },
            {
                "id": 3,
                "texto": "As áreas têm autonomia para sugerir melhorias nos processos?",
                "opcoes": [
                    "Não há espaço para sugestões e mudanças.",
                    "Sugestões ocorrem esporadicamente, mas raramente são implementadas.",
                    "Algumas áreas têm autonomia, mas há burocracia para mudanças.",
                    "Existe um canal claro para propostas de melhorias.",
                    "Cultura de melhoria contínua e incentivo à inovação nos processos."
                ]
            },
            {
                "id": 4,
                "texto": "Os processos são integrados entre as áreas para evitar retrabalho?",
                "opcoes": [
                    "Cada área opera isoladamente, sem integração.",
                    "Algumas áreas compartilham informações, mas sem padronização.",
                    "Processos são integrados em algumas áreas, mas ainda há falhas.",
                    "A empresa tem um modelo estruturado de integração entre setores.",
                    "Processos altamente conectados, com colaboração fluida entre áreas."
                ]
            },
            {
                "id": 5,
                "texto": "Os processos estão alinhados às necessidades do cliente e garantem boa experiência?",
                "opcoes": [
                    "Processos não consideram a experiência do cliente.",
                    "Há algumas práticas voltadas ao cliente, mas sem estrutura.",
                    "Existem processos, mas nem sempre são eficazes para garantir uma boa experiência.",
                    "Processos bem definidos para atender às expectativas dos clientes.",
                    "A empresa tem uma abordagem centrada no cliente e revisa processos constantemente."
                ]
            }
        ]
    },
    {
        "sessao": 2,
        "titulo": "Governança, Gestão Estratégica e Tomada de Decisão",
        "perguntas": [
            {
                "id": 6,
                "texto": "As decisões estratégicas são baseadas em dados e indicadores?",
                "opcoes": [
                    "Decisões são feitas com base em intuição e experiências passadas.",
                    "Alguns dados são analisados, mas não influenciam as decisões.",
                    "Indicadores são usados ocasionalmente, mas sem análise aprofundada.",
                    "Decisões orientadas por dados e métricas claras.",
                    "Estratégia 100% baseada em dados, com análises preditivas."
                ]
            },
            {
                "id": 7,
                "texto": "A empresa possui uma estrutura organizacional clara (organograma), com papéis bem definidos?",
                "opcoes": [
                    "Papéis e responsabilidades não são definidos formalmente.",
                    "Algumas áreas possuem estrutura clara, mas há sobreposição de funções.",
                    "Estrutura organizacional documentada, mas com lacunas operacionais.",
                    "Estrutura bem definida e de fácil compreensão para todos.",
                    "Estrutura robusta, alinhada ao crescimento e visão da empresa."
                ]
            },
            {
                "id": 8,
                "texto": "A empresa possui um escritório de projetos (PMO) ou metodologia estruturada para gestão de projetos?",
                "opcoes": [
                    "A empresa não possui um PMO nem metodologias padronizadas para gestão de projetos.",
                    "Algumas metodologias são aplicadas, mas sem padronização entre áreas.",
                    "Existe uma abordagem formal para gestão de projetos, com algumas diretrizes documentadas, mas a adoção ainda é parcial.",
                    "A empresa tem um PMO estruturado ou uma metodologia bem definida, garantindo padronização e alinhamento estratégico.",
                    "O gerenciamento de projetos é altamente maduro, com um PMO consolidado e alinhamento total com a estratégia do negócio."
                ]
            },
            {
                "id": 9,
                "texto": "Existe um planejamento estratégico documentado e acompanhado regularmente?",
                "opcoes": [
                    "Não há planejamento estratégico formal.",
                    "O planejamento existe, mas as metas não são bem definidas.",
                    "O planejamento existe, com metas, mas pouco acompanhado na prática.",
                    "Planejamento revisado periodicamente, com metas monitoradas.",
                    "Estratégia bem definida, monitorada continuamente e ajustada conforme necessário."
                ]
            },
            {
                "id": 10,
                "texto": "A empresa possui um modelo de compliance e ética bem definidos?",
                "opcoes": [
                    "Não há um código de ética ou modelo de compliance.",
                    "Algumas diretrizes existem, mas são poucos seguidas.",
                    "O modelo de compliance está estruturado, mas carece de aplicação rigorosa.",
                    "Compliance bem definido e implementado na empresa.",
                    "A empresa tem uma governança sólida, com alto nível de integridade e transparência."
                ]
            }
        ]
    },
    {
        "sessao": 3,
        "titulo": "Cultura e Liderança",
        "perguntas": [
            {
                "id": 11,
                "texto": "A cultura organizacional está bem definida e integrada ao dia a dia da empresa?",
                "opcoes": [
                    "A empresa não possui valores organizacionais claros e não há diretrizes formais sobre cultura.",
                    "Os valores existem, mas não são comunicados de forma estruturada, e a cultura é pouco perceptível no dia a dia.",
                    "A cultura organizacional está documentada e divulgada, mas ainda não é totalmente praticada por todos.",
                    "A cultura está integrada aos processos e práticas da empresa, sendo reconhecida e vivenciada pelos colaboradores.",
                    "A cultura organizacional é um pilar estratégico da empresa, influenciando todas as decisões e sendo um diferencial competitivo."
                ]
            },
            {
                "id": 12,
                "texto": "Os gestores incentivam e promovem a colaboração entre equipes?",
                "opcoes": [
                    "Não há incentivo para colaboração entre setores.",
                    "Colaboração ocorre ocasionalmente, mas sem estrutura definida.",
                    "Existem algumas práticas colaborativas, mas ainda há silos organizacionais.",
                    "A colaboração é incentivada e há processos claros para interação entre equipes.",
                    "A empresa tem uma cultura forte de colaboração, inovação e trabalho em equipe."
                ]
            },
            {
                "id": 13,
                "texto": "A empresa oferece treinamentos e capacitação para líderes?",
                "opcoes": [
                    "Não há programas de desenvolvimento para líderes.",
                    "Capacitações acontecem de forma esporádica e sem continuidade.",
                    "Há programas de desenvolvimento, mas poucos líderes participam.",
                    "A empresa oferece treinamentos regulares para desenvolvimento da liderança.",
                    "O desenvolvimento de líderes é contínuo e estratégico, com acompanhamento e métricas de evolução."
                ]
            },
            {
                "id": 14,
                "texto": "A empresa possui canais abertos para ouvir sugestões e feedbacks dos colaboradores?",
                "opcoes": [
                    "Não há canais formais para coleta de feedbacks.",
                    "Há formas de dar feedback, mas poucas ações concretas resultam disso.",
                    "A empresa coleta feedbacks, mas nem sempre há retorno sobre as sugestões.",
                    "Há um sistema estruturado de feedback, com acompanhamento e ações claras.",
                    "Cultura aberta ao diálogo, com participação ativa dos colaboradores em decisões estratégicas."
                ]
            },
            {
                "id": 15,
                "texto": "A comunicação interna é estruturada e promove alinhamento entre times?",
                "opcoes": [
                    "Não há uma estratégia formal de comunicação interna, e as informações são transmitidas de forma desorganizada e inconsistente.",
                    "Existem alguns canais de comunicação interna, mas a troca de informações ainda é falha e gera ruídos entre as equipes.",
                    "A comunicação interna é estruturada e ocorre regularmente, mas nem sempre é eficaz na disseminação de informações estratégicas.",
                    "A empresa possui canais bem definidos e estruturados para comunicação interna, promovendo alinhamento e transparência entre as áreas.",
                    "A comunicação interna é altamente eficaz, integrada a processos estratégicos e promove um ambiente colaborativo, garantindo alinhamento total entre equipes e liderança."
                ]
            }
        ]
    },
    {
        "sessao": 4,
        "titulo": "Gestão de Pessoas",
        "perguntas": [
            {
                "id": 16,
                "texto": "A empresa possui um plano estruturado de desenvolvimento de talentos?",
                "opcoes": [
                    "Não há plano de desenvolvimento.",
                    "Algumas iniciativas existem, mas sem estruturação clara.",
                    "Plano estruturado, mas pouco implementado.",
                    "Plano de desenvolvimento ativo e aplicado regularmente.",
                    "O desenvolvimento de talentos é contínuo e integrado à estratégia da empresa."
                ]
            },
            {
                "id": 17,
                "texto": "A empresa tem uma política clara de promoções e crescimento interno?",
                "opcoes": [
                    "Não há critérios definidos para promoções.",
                    "Promoções ocorrem, mas sem transparência ou critério claro.",
                    "Existem critérios formais, mas nem sempre são seguidos.",
                    "Há uma política clara de crescimento, aplicada de forma justa.",
                    "Plano de carreira estruturado, promovendo crescimento e retenção de talentos."
                ]
            },
            {
                "id": 18,
                "texto": "O processo de recrutamento e seleção garante o fit cultural e técnico dos novos talentos?",
                "opcoes": [
                    "Não há critérios claros para contratação.",
                    "As contratações são feitas sem um processo estruturado.",
                    "Existe um processo, mas ainda há erros na escolha de talentos.",
                    "O recrutamento é bem estruturado e alinhado à cultura da empresa.",
                    "Processo de seleção altamente eficiente, garantindo alinhamento cultural e técnico."
                ]
            },
            {
                "id": 19,
                "texto": "A empresa realiza avaliações de desempenho e feedback contínuo?",
                "opcoes": [
                    "Não há avaliação de desempenho formal.",
                    "Avaliações acontecem, mas sem regularidade.",
                    "Existem avaliações estruturadas, mas nem sempre são seguidas de ações.",
                    "Feedback contínuo, com avaliações estruturadas e acompanhamento.",
                    "Cultura de performance bem definida, com ações concretas de desenvolvimento."
                ]
            },
            {
                "id": 20,
                "texto": "A empresa realiza pesquisas de clima organizacional e toma ações baseadas nos resultados?",
                "opcoes": [
                    "Nunca realizou pesquisas de clima.",
                    "Já realizou, mas sem ações concretas baseadas nos resultados.",
                    "Pesquisa aplicada, mas com poucas ações de melhoria.",
                    "Pesquisa regular, com melhorias implementadas.",
                    "Pesquisa estratégica, orientando decisões e melhorias contínuas."
                ]
            }
        ]
    },
    {
        "sessao": 5,
        "titulo": "Inovação e Tecnologia",
        "perguntas": [
            {
                "id": 21,
                "texto": "A empresa investe regularmente em tecnologia para otimizar processos?",
                "opcoes": [
                    "A empresa não investe em tecnologia e utiliza processos manuais.",
                    "Algumas áreas usam tecnologia, mas de forma limitada e desorganizada.",
                    "A empresa investe ocasionalmente, mas sem um planejamento estratégico.",
                    "Há investimentos contínuos, com tecnologia aplicada para eficiência.",
                    "Tecnologia altamente integrada, com otimização e inovação constantes."
                ]
            },
            {
                "id": 22,
                "texto": "Há incentivo à inovação e à busca por novas soluções dentro da empresa?",
                "opcoes": [
                    "A inovação não é incentivada e a empresa mantém processos tradicionais.",
                    "Há iniciativas esporádicas, mas sem uma cultura de inovação estruturada.",
                    "Algumas áreas promovem inovação, mas sem alinhamento geral na empresa.",
                    "A inovação faz parte da estratégia e há incentivo ao intraempreendedorismo.",
                    "Cultura de inovação consolidada, com investimentos frequentes e suporte a novas ideias."
                ]
            },
            {
                "id": 23,
                "texto": "A empresa adota metodologias ágeis para desenvolvimento e inovação?",
                "opcoes": [
                    "Não há metodologias definidas e o desenvolvimento ocorre de forma desorganizada.",
                    "Algumas áreas adotam metodologias, mas sem padronização.",
                    "Há processos ágeis aplicados, mas ainda com resistência interna.",
                    "A empresa utiliza metodologias ágeis em diversas áreas e obtém bons resultados.",
                    "Metodologias ágeis fazem parte do DNA da empresa e são aplicadas com excelência."
                ]
            },
            {
                "id": 24,
                "texto": "Os colaboradores são incentivados a propor novas ideias e melhorias?",
                "opcoes": [
                    "A empresa não tem um canal ou prática para ouvir sugestões.",
                    "Há espaços informais para sugestões, mas sem impacto real.",
                    "Algumas sugestões são consideradas, mas a implementação é lenta.",
                    "Há um processo estruturado para avaliação e implementação de novas ideias.",
                    "A empresa possui um programa ativo de inovação com reconhecimento e premiações."
                ]
            },
            {
                "id": 25,
                "texto": "A empresa acompanha tendências tecnológicas do setor e adapta-se rapidamente?",
                "opcoes": [
                    "A empresa não monitora novas tecnologias e continua operando com sistemas antigos.",
                    "Há um interesse em novas tendências, mas pouca aplicação prática.",
                    "Algumas mudanças tecnológicas são implementadas, mas sem uma estratégia clara.",
                    "A empresa monitora tendências e implementa tecnologias relevantes.",
                    "A empresa está sempre à frente do mercado, adotando inovações com rapidez."
                ]
            }
        ]
    },
    {
        "sessao": 6,
        "titulo": "Resultados e Performance",
        "perguntas": [
            {
                "id": 26,
                "texto": "A empresa possui KPIs bem definidos para medir desempenho e crescimento?",
                "opcoes": [
                    "Não há indicadores de desempenho estabelecidos.",
                    "Algumas métricas são monitoradas, mas sem padronização.",
                    "KPIs são definidos, mas nem sempre analisados de forma estratégica.",
                    "KPIs bem estabelecidos, com acompanhamento regular.",
                    "A empresa tem KPIs estratégicos que guiam decisões e promovem crescimento sustentável."
                ]
            },
            {
                "id": 27,
                "texto": "Os resultados financeiros são analisados regularmente?",
                "opcoes": [
                    "Não há análise financeira estruturada.",
                    "Relatórios financeiros são gerados, mas sem impacto na gestão.",
                    "A análise financeira ocorre, mas sem consistência e acompanhamento detalhado.",
                    "A empresa faz análises regulares e usa os dados para otimizar a performance.",
                    "Gestão financeira altamente estratégica, com previsões baseadas em dados sólidos."
                ]
            },
            {
                "id": 28,
                "texto": "As metas da empresa são comunicadas e alinhadas entre todas as áreas?",
                "opcoes": [
                    "As metas não são formalmente definidas e cada área trabalha de forma isolada.",
                    "Algumas metas são compartilhadas, mas sem alinhamento claro entre as equipes.",
                    "Há metas organizacionais, mas falta conexão entre áreas e execução prática.",
                    "As metas são bem estruturadas e todos os setores trabalham em conjunto para atingi-las.",
                    "A empresa tem um planejamento estratégico sólido e metas bem comunicadas, alinhadas à visão de longo prazo."
                ]
            },
            {
                "id": 29,
                "texto": "Existe um modelo estruturado para avaliar a produtividade dos colaboradores?",
                "opcoes": [
                    "Não há métricas para avaliar a produtividade dos times.",
                    "A produtividade é avaliada de forma subjetiva, sem indicadores claros.",
                    "Algumas métricas existem, mas não são usadas de forma eficiente.",
                    "A produtividade é monitorada regularmente e usada para otimização dos processos.",
                    "A empresa possui um modelo estruturado, com análise de dados para maximizar a eficiência operacional."
                ]
            },
            {
                "id": 30,
                "texto": "Os gestores revisam os resultados e tomam ações estratégicas de forma proativa?",
                "opcoes": [
                    "A empresa não revisa os resultados regularmente.",
                    "Há revisões esporádicas, mas sem os dados para análise do impacto real nas estratégias.",
                    "Os gestores analisam os resultados, mas sem um plano de ação estruturado.",
                    "Revisões são frequentes e orientam ações estratégicas concretas.",
                    "A empresa opera de forma altamente estratégica, tomando decisões ágeis e baseadas em dados."
                ]
            }
        ]
    }
]

NIVEIS = [
    {"min": 1.0, "max": 1.9, "nivel": 1, "nome": "Inicial", "subtitulo": "Baixa Maturidade — Operação Reativa", "cor": "#e53935",
     "caracteristicas": [
         "Processos inexistentes ou informais, sem documentação e sem padronização.",
         "Tomada de decisão baseada em intuição, sem indicadores claros.",
         "Liderança centralizada nos sócios ou poucos gestores, dificultando a delegação.",
         "Falta de previsibilidade nos resultados financeiros e operacionais.",
         "Problemas frequentes de comunicação e desalinhamento interno."
     ],
     "desafios": [
         "Falta de estrutura organizacional.",
         "Retrabalho constante e baixa eficiência operacional.",
         "Turnover elevado e dificuldade de retenção de talentos."
     ],
     "acoes": [
         "Estruturar processos básicos e padronizar fluxos de trabalho.",
         "Definir papéis e responsabilidades com maior clareza.",
         "Criar primeiros indicadores para mensurar resultados."
     ]},
    {"min": 2.0, "max": 2.9, "nivel": 2, "nome": "Em Desenvolvimento", "subtitulo": "Estruturação Inicial", "cor": "#fb8c00",
     "caracteristicas": [
         "Algumas áreas possuem processos organizados, mas ainda há inconsistências.",
         "Decisões começam a ser baseadas em dados, mas sem um monitoramento constante.",
         "Há um esforço para fortalecer a cultura organizacional, mas nem todos os colaboradores a vivenciam.",
         "Líderes em desenvolvimento, mas sem um modelo estruturado de gestão.",
         "Pouca inovação e resistência a mudanças dentro da empresa."
     ],
     "desafios": [
         "Processos não são seguidos por toda a equipe.",
         "Falta de métricas claras para avaliar desempenho.",
         "Dificuldade em escalar a empresa de forma sustentável."
     ],
     "acoes": [
         "Formalizar processos e garantir que sejam seguidos por toda a equipe.",
         "Implementar reuniões de alinhamento entre áreas para evitar falhas de comunicação.",
         "Criar um plano de desenvolvimento para a liderança e estruturar feedbacks contínuos."
     ]},
    {"min": 3.0, "max": 3.9, "nivel": 3, "nome": "Estabelecido", "subtitulo": "Gestão Estruturada", "cor": "#fdd835",
     "caracteristicas": [
         "Processos bem definidos, documentados e seguidos de forma consistente.",
         "Decisões cada vez mais baseadas em métricas e indicadores estratégicos.",
         "Cultura organizacional bem disseminada e reconhecida pelos colaboradores.",
         "Líderes atuam de forma estruturada, com feedbacks regulares e desenvolvimento do time.",
         "Melhor previsibilidade financeira e maior controle dos custos."
     ],
     "desafios": [
         "Manter a cultura organizacional forte durante o crescimento.",
         "Garantir que processos não se tornem burocráticos.",
         "Criar um ambiente que incentive inovação e melhoria contínua."
     ],
     "acoes": [
         "Fortalecer o uso de tecnologia para automatizar processos e ganhar eficiência.",
         "Ampliar o desenvolvimento de lideranças para garantir um time maduro e preparado para escalar.",
         "Estruturar um modelo de gestão por indicadores (KPIs) para tomada de decisão mais assertiva."
     ]},
    {"min": 4.0, "max": 4.4, "nivel": 4, "nome": "Avançado", "subtitulo": "Gestão Estratégica e Inovação", "cor": "#43a047",
     "caracteristicas": [
         "A empresa opera de forma estratégica, com processos bem estruturados e tecnologia integrada.",
         "Tomada de decisão 100% baseada em dados, com análise preditiva.",
         "Cultura organizacional consolidada e valorizada no mercado.",
         "Lideranças maduras e bem preparadas, atuando de forma descentralizada.",
         "Inovação e melhoria contínua fazem parte da rotina organizacional."
     ],
     "desafios": [
         "Manter a empresa ágil e inovadora mesmo com crescimento acelerado.",
         "Atrair e reter talentos altamente qualificados.",
         "Expandir mercado sem perder a qualidade e a identidade da marca."
     ],
     "acoes": [
         "Criar programas contínuos de inovação e intraempreendedorismo.",
         "Investir na capacitação dos colaboradores para manter a empresa competitiva.",
         "Desenvolver estratégias de crescimento sustentável e escalável."
     ]},
    {"min": 4.5, "max": 5.0, "nivel": 5, "nome": "Excelência", "subtitulo": "Empresa Referência no Mercado", "cor": "#1565c0",
     "caracteristicas": [
         "Gestão extremamente eficiente, inovadora e orientada por dados.",
         "Modelos preditivos e inteligência artificial auxiliam na tomada de decisão.",
         "Empresa reconhecida como referência no setor (benchmark para outras organizações).",
         "Equipe altamente engajada, motivada e produtiva.",
         "Cultura de aprendizado contínuo, inovação e adaptação a mudanças."
     ],
     "desafios": [
         "Manter a competitividade diante de novas tendências e concorrência.",
         "Sustentar a cultura organizacional mesmo com grande crescimento.",
         "Expandir internacionalmente sem comprometer a eficiência."
     ],
     "acoes": [
         "Criar um modelo de governança corporativa de alto nível.",
         "Explorar novos mercados e diversificar produtos/serviços.",
         "Investir fortemente em inovação e pesquisa para manter liderança no setor."
     ]}
]

# Recomendações de mercado por categoria e faixa de score
RECOMENDACOES = {
    "Estrutura e Processos": {
        "baixo": [
            "Implementar um mapeamento de processos (BPM) usando ferramentas como Bizagi ou Lucidchart para documentar fluxos de trabalho críticos.",
            "Adotar a metodologia Lean para eliminar desperdícios e padronizar operações essenciais.",
            "Criar um repositório centralizado de SOPs (Standard Operating Procedures) acessível a todos os colaboradores.",
            "Realizar workshops de process mining para identificar gargalos e redundâncias nos fluxos atuais."
        ],
        "medio": [
            "Implementar um sistema de gestão por processos (BPMS) para automatizar fluxos e monitorar SLAs internos.",
            "Adotar certificações como ISO 9001 para garantir padronização e melhoria contínua dos processos.",
            "Utilizar ferramentas de RPA (Robotic Process Automation) para automatizar tarefas repetitivas e reduzir erros.",
            "Criar comitês de melhoria contínua com representantes de cada área para integração entre departamentos."
        ],
        "alto": [
            "Implementar Process Mining com ferramentas como Celonis para análise avançada de eficiência operacional.",
            "Adotar práticas de Digital Twin de processos para simulação e otimização preditiva.",
            "Investir em hiperautomação combinando IA, RPA e low-code para orquestração inteligente de processos.",
            "Implementar OKRs (Objectives and Key Results) integrados aos processos para alinhamento estratégico contínuo."
        ]
    },
    "Governança, Gestão Estratégica e Tomada de Decisão": {
        "baixo": [
            "Estabelecer um planejamento estratégico básico usando a metodologia SWOT e definir metas SMART para os próximos 12 meses.",
            "Criar um organograma funcional claro e documentar as responsabilidades de cada posição (matriz RACI).",
            "Implementar reuniões mensais de acompanhamento estratégico com indicadores básicos de desempenho.",
            "Desenvolver um código de ética e conduta como base para a governança corporativa."
        ],
        "medio": [
            "Implementar um BSC (Balanced Scorecard) para traduzir a estratégia em indicadores operacionais mensuráveis.",
            "Adotar ferramentas de BI (Business Intelligence) como Power BI ou Tableau para dashboards de tomada de decisão.",
            "Estruturar um PMO (Project Management Office) para padronizar a gestão de projetos e iniciativas estratégicas.",
            "Implementar framework de gestão de riscos corporativos baseado em COSO ou ISO 31000."
        ],
        "alto": [
            "Implementar People Analytics e análise preditiva para antecipar tendências e suportar decisões estratégicas.",
            "Adotar modelos de governança ágil (Agile Governance) para maior velocidade na tomada de decisão.",
            "Utilizar inteligência artificial para análise de cenários estratégicos e simulações de mercado.",
            "Estruturar um conselho consultivo com especialistas externos para ampliar a visão estratégica."
        ]
    },
    "Cultura e Liderança": {
        "baixo": [
            "Definir e comunicar formalmente os valores, missão e visão da empresa a todos os colaboradores.",
            "Implementar um programa básico de desenvolvimento de liderança com treinamentos mensais.",
            "Criar canais de comunicação interna estruturados (ex: newsletter semanal, murais digitais, reuniões all-hands).",
            "Realizar uma pesquisa de clima organizacional para diagnosticar a percepção dos colaboradores."
        ],
        "medio": [
            "Implementar programas de mentoria e coaching executivo para desenvolvimento de líderes intermediários.",
            "Adotar metodologias de feedback contínuo como 1:1s estruturados e feedforward.",
            "Criar programas de embaixadores da cultura para disseminar e fortalecer valores organizacionais.",
            "Implementar ferramentas de comunicação colaborativa (Slack, Teams) com rituais de alinhamento definidos."
        ],
        "alto": [
            "Implementar programas de liderança transformacional com assessment 360 graus e planos individuais.",
            "Adotar práticas de Psychological Safety (Segurança Psicológica) baseadas nas pesquisas do Google Project Aristotle.",
            "Criar um programa de inovação cultural com hackathons internos e laboratórios de experimentação.",
            "Desenvolver uma universidade corporativa com trilhas de liderança e cultura alinhadas à estratégia."
        ]
    },
    "Gestão de Pessoas": {
        "baixo": [
            "Implementar um processo estruturado de onboarding para novos colaboradores (primeiros 90 dias).",
            "Criar descrições de cargo e competências mínimas para cada posição, estabelecendo base para avaliações.",
            "Adotar uma ferramenta de gestão de RH (ex: Gupy, Kenoby) para estruturar recrutamento e seleção.",
            "Implementar avaliações de desempenho semestrais com critérios claros e devolutiva individual."
        ],
        "medio": [
            "Implementar um programa de PDI (Plano de Desenvolvimento Individual) integrado ao assessment DISC ou MBTI.",
            "Criar trilhas de carreira estruturadas com critérios transparentes de promoção e movimentação interna.",
            "Adotar plataformas de gestão de talentos (ex: SAP SuccessFactors, Workday) para visão integrada do ciclo de pessoas.",
            "Implementar pesquisas de engajamento trimestrais com planos de ação por área."
        ],
        "alto": [
            "Implementar People Analytics avançado para prever turnover, identificar talentos de alto potencial e otimizar alocação.",
            "Criar programas de Employee Experience Design com jornadas mapeadas e métricas de NPS interno.",
            "Adotar modelos de remuneração variável atrelados a OKRs e performance individual/coletiva.",
            "Implementar programas de sucessão com pipeline de liderança e desenvolvimento acelerado de talentos."
        ]
    },
    "Inovação e Tecnologia": {
        "baixo": [
            "Realizar um diagnóstico de maturidade digital para identificar oportunidades de digitalização prioritárias.",
            "Adotar ferramentas de produtividade em nuvem (Google Workspace ou Microsoft 365) para colaboração básica.",
            "Implementar um ERP ou sistema de gestão integrado para centralizar informações operacionais e financeiras.",
            "Criar um comitê de inovação para avaliar e priorizar iniciativas de melhoria tecnológica."
        ],
        "medio": [
            "Implementar metodologias ágeis (Scrum, Kanban) em projetos de inovação e desenvolvimento.",
            "Adotar plataformas low-code/no-code para acelerar a digitalização de processos internos.",
            "Criar um programa de intraempreendedorismo para estimular ideias inovadoras dos colaboradores.",
            "Investir em automação de processos com RPA e integrações via APIs entre sistemas existentes."
        ],
        "alto": [
            "Implementar IA generativa e machine learning para otimização de processos e tomada de decisão.",
            "Criar um laboratório de inovação (Innovation Lab) para testar novas tecnologias e modelos de negócio.",
            "Adotar práticas de DevOps e CI/CD para entrega contínua de soluções tecnológicas.",
            "Investir em parcerias com startups e ecossistemas de inovação para co-criação de soluções disruptivas."
        ]
    },
    "Resultados e Performance": {
        "baixo": [
            "Definir KPIs básicos por área (financeiro, comercial, operacional, pessoas) e criar rotina de acompanhamento mensal.",
            "Implementar um sistema de gestão financeira estruturado com DRE, fluxo de caixa e projeções básicas.",
            "Criar reuniões de resultado mensais com apresentação de indicadores e planos de ação.",
            "Estabelecer metas departamentais alinhadas aos objetivos gerais da empresa."
        ],
        "medio": [
            "Implementar dashboards de performance em tempo real com ferramentas de BI para visibilidade cross-funcional.",
            "Adotar a metodologia OKR (Objectives and Key Results) para alinhar metas individuais à estratégia organizacional.",
            "Criar um modelo de gestão de performance com ciclos trimestrais de revisão e calibração.",
            "Implementar análise de produtividade por equipe com métricas objetivas e benchmarks de mercado."
        ],
        "alto": [
            "Implementar analytics preditivo para projeção de receita, churn e tendências de mercado.",
            "Adotar modelos de gestão de valor (EVA, ROIC) para decisões de investimento e alocação de recursos.",
            "Criar um sistema integrado de performance management com cascateamento estratégico end-to-end.",
            "Implementar benchmarking contínuo com empresas referência do setor para melhoria constante."
        ]
    }
}


def get_recomendacoes(scores_sessao):
    """Gera recomendações por categoria baseado no score de cada sessão."""
    resultado = {}
    sessao_titulos = {
        1: "Estrutura e Processos",
        2: "Governança, Gestão Estratégica e Tomada de Decisão",
        3: "Cultura e Liderança",
        4: "Gestão de Pessoas",
        5: "Inovação e Tecnologia",
        6: "Resultados e Performance"
    }
    for sessao_key, dados in scores_sessao.items():
        s = int(sessao_key)
        titulo = sessao_titulos.get(s, dados.get("titulo", ""))
        media = dados["media"]
        if media < 2.5:
            faixa = "baixo"
        elif media < 4.0:
            faixa = "medio"
        else:
            faixa = "alto"
        recs = RECOMENDACOES.get(titulo, {}).get(faixa, [])
        resultado[str(s)] = {"titulo": titulo, "media": media, "faixa": faixa, "recomendacoes": recs}
    return resultado


def init_db():
    conn = sqlite3.connect(DB_PATH)
    c = conn.cursor()
    c.execute('''CREATE TABLE IF NOT EXISTS respostas (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        token TEXT UNIQUE NOT NULL,
        nome TEXT NOT NULL,
        email TEXT NOT NULL DEFAULT '',
        empresa TEXT NOT NULL,
        respostas TEXT NOT NULL,
        scores_sessao TEXT NOT NULL,
        score_total REAL NOT NULL,
        nivel INTEGER NOT NULL,
        created_at TEXT NOT NULL
    )''')
    # Ensure email column exists for existing databases
    try:
        c.execute('ALTER TABLE respostas ADD COLUMN email TEXT NOT NULL DEFAULT ""')
    except sqlite3.OperationalError:
        pass  # Column already exists
    conn.commit()
    conn.close()


def build_email_html(nome, empresa, score_total, nivel_info, scores_sessao, recomendacoes, token, base_url):
    """Gera o HTML do e-mail de resultado."""
    nivel_cores = {1: '#e53935', 2: '#fb8c00', 3: '#fdd835', 4: '#43a047', 5: '#1565c0'}
    cor = nivel_cores.get(nivel_info['nivel'], '#0097a7')
    text_color = '#333' if nivel_info['nivel'] == 3 else '#fff'

    sessao_rows = ''
    sorted_keys = sorted(scores_sessao.keys(), key=lambda x: int(x))
    for k in sorted_keys:
        s = scores_sessao[k]
        bar_pct = int((s['media'] / 5) * 100)
        sessao_rows += f'''<tr>
            <td style="padding:8px 12px; border-bottom:1px solid #eee; font-size:14px;">{s['titulo']}</td>
            <td style="padding:8px 12px; border-bottom:1px solid #eee; text-align:center; font-weight:700; font-size:16px; color:#0097a7;">{s['media']:.1f}</td>
            <td style="padding:8px 12px; border-bottom:1px solid #eee; width:120px;">
                <div style="background:#e0e0e0; border-radius:4px; height:8px; overflow:hidden;">
                    <div style="background:#0097a7; width:{bar_pct}%; height:100%; border-radius:4px;"></div>
                </div>
            </td>
        </tr>'''

    rec_sections = ''
    for k in sorted_keys:
        rec = recomendacoes.get(k, {})
        titulo = rec.get('titulo', '')
        recs = rec.get('recomendacoes', [])
        if recs:
            items = ''.join(f'<li style="padding:4px 0; font-size:13px; color:#455a64; line-height:1.5;">{r}</li>' for r in recs)
            rec_sections += f'''<div style="margin-bottom:16px;">
                <h4 style="color:#0097a7; font-size:14px; margin:0 0 8px 0; text-transform:uppercase; letter-spacing:1px;">{titulo}</h4>
                <ul style="margin:0; padding-left:20px;">{items}</ul>
            </div>'''

    result_url = f'{base_url}/resultado/{token}'

    return f'''<!DOCTYPE html>
<html><head><meta charset="UTF-8"></head>
<body style="margin:0; padding:0; font-family:'Segoe UI',system-ui,sans-serif; background:#f5f7fa;">
<div style="max-width:600px; margin:0 auto; padding:20px;">

    <div style="background:linear-gradient(135deg,#0a1628,#0d2137); padding:32px; text-align:center; border-radius:16px 16px 0 0;">
        <div style="font-size:28px; font-weight:800; color:#00bcd4; letter-spacing:3px;">CIH</div>
        <div style="font-size:11px; color:#607d8b; letter-spacing:4px; text-transform:uppercase; margin-top:4px;">Consultoria Impacto Humano</div>
        <h1 style="color:#fff; font-size:16px; font-weight:400; letter-spacing:2px; margin-top:12px;">RESULTADO DA AVALIAÇÃO DE MATURIDADE</h1>
    </div>

    <div style="background:#fff; padding:32px; border-radius:0 0 16px 16px; box-shadow:0 2px 8px rgba(0,0,0,0.06);">

        <p style="color:#607d8b; font-size:14px;">Olá <strong style="color:#263238;">{nome}</strong>,</p>
        <p style="color:#607d8b; font-size:14px;">Segue o resultado da avaliação de maturidade organizacional da empresa <strong style="color:#263238;">{empresa}</strong>.</p>

        <div style="text-align:center; margin:24px 0;">
            <div style="display:inline-block; background:{cor}; color:{text_color}; padding:20px 40px; border-radius:16px;">
                <div style="font-size:36px; font-weight:800; line-height:1;">{score_total:.2f}</div>
                <div style="font-size:13px; margin-top:6px; font-weight:600; text-transform:uppercase; letter-spacing:1px;">Nível {nivel_info['nivel']} — {nivel_info['nome']}</div>
            </div>
        </div>

        <h3 style="color:#263238; font-size:16px; margin:24px 0 12px;">Score por Dimensão</h3>
        <table style="width:100%; border-collapse:collapse;">{sessao_rows}</table>

        <h3 style="color:#263238; font-size:16px; margin:28px 0 16px;">Recomendações por Categoria</h3>
        {rec_sections}

        <div style="text-align:center; margin:32px 0 16px;">
            <a href="{result_url}" style="display:inline-block; background:linear-gradient(135deg,#0097a7,#00bcd4); color:#fff; padding:14px 32px; border-radius:10px; text-decoration:none; font-weight:600; font-size:14px;">Ver Resultado Completo</a>
        </div>

        <p style="color:#607d8b; font-size:12px; text-align:center; margin-top:8px;">Token de acesso: <strong style="font-family:monospace; letter-spacing:2px;">{token}</strong></p>

    </div>

    <div style="text-align:center; padding:24px; color:#607d8b; font-size:12px;">
        <p><strong>CIH</strong> — Consultoria Impacto Humano</p>
        <p>Soluções Inteligentes para Gestão de Pessoas</p>
    </div>

</div>
</body></html>'''


def send_result_email(to_email, nome, empresa, score_total, nivel_info, scores_sessao, recomendacoes, token, base_url):
    """Envia o e-mail com o resultado usando SMTP."""
    smtp_host = os.environ.get('SMTP_HOST', '')
    smtp_port = int(os.environ.get('SMTP_PORT', '587'))
    smtp_user = os.environ.get('SMTP_USER', '')
    smtp_pass = os.environ.get('SMTP_PASS', '')
    smtp_from = os.environ.get('SMTP_FROM', smtp_user)

    if not smtp_host or not smtp_user:
        return False, "SMTP não configurado"

    html = build_email_html(nome, empresa, score_total, nivel_info, scores_sessao, recomendacoes, token, base_url)

    msg = MIMEMultipart('alternative')
    msg['Subject'] = f'Resultado Maturidade Organizacional — {empresa} | CIH'
    msg['From'] = f'CIH - Consultoria Impacto Humano <{smtp_from}>'
    msg['To'] = to_email
    msg.attach(MIMEText(html, 'html', 'utf-8'))

    try:
        server = smtplib.SMTP(smtp_host, smtp_port)
        server.ehlo()
        if smtp_port != 25:
            server.starttls()
        server.login(smtp_user, smtp_pass)
        server.sendmail(smtp_from, [to_email], msg.as_string())
        server.quit()
        return True, "OK"
    except Exception as e:
        return False, str(e)


def calcular_resultado(respostas_dict):
    scores_sessao = {}
    for sessao in QUESTOES:
        s = sessao["sessao"]
        titulo = sessao["titulo"]
        total = 0
        count = 0
        for p in sessao["perguntas"]:
            pid = str(p["id"])
            if pid in respostas_dict:
                total += int(respostas_dict[pid])
                count += 1
        media = round(total / count, 2) if count > 0 else 0
        scores_sessao[s] = {"titulo": titulo, "media": media}

    soma = sum(v["media"] for v in scores_sessao.values())
    score_total = round(soma / len(scores_sessao), 2)

    nivel_resultado = NIVEIS[0]
    for n in NIVEIS:
        if n["min"] <= score_total <= n["max"]:
            nivel_resultado = n
            break

    return scores_sessao, score_total, nivel_resultado


@app.route('/')
def index():
    return send_from_directory('static', 'index.html')


@app.route('/resultado/<token>')
def resultado_page(token):
    return send_from_directory('static', 'resultado.html')


@app.route('/api/questoes', methods=['GET'])
def get_questoes():
    return jsonify(QUESTOES)


@app.route('/api/enviar', methods=['POST'])
def enviar():
    data = request.json
    nome = data.get('nome', '').strip()
    email = data.get('email', '').strip()
    empresa = data.get('empresa', '').strip()
    respostas = data.get('respostas', {})

    if not nome or not empresa or not email:
        return jsonify({"error": "Nome, e-mail e empresa são obrigatórios."}), 400
    if len(respostas) < 30:
        return jsonify({"error": "Responda todas as 30 perguntas."}), 400

    scores_sessao, score_total, nivel = calcular_resultado(respostas)
    token = uuid.uuid4().hex[:12].upper()

    scores_sessao_str = {str(k): v for k, v in scores_sessao.items()}
    recomendacoes = get_recomendacoes(scores_sessao_str)

    conn = sqlite3.connect(DB_PATH)
    c = conn.cursor()
    c.execute('''INSERT INTO respostas (token, nome, email, empresa, respostas, scores_sessao, score_total, nivel, created_at)
                 VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)''',
              (token, nome, email, empresa, json.dumps(respostas), json.dumps(scores_sessao_str),
               score_total, nivel["nivel"], datetime.now().isoformat()))
    conn.commit()
    conn.close()

    # Send email in background (non-blocking)
    base_url = request.host_url.rstrip('/')
    email_sent = False
    email_error = ""
    try:
        ok, msg = send_result_email(email, nome, empresa, score_total, nivel, scores_sessao_str, recomendacoes, token, base_url)
        email_sent = ok
        email_error = msg
    except Exception as e:
        email_error = str(e)

    return jsonify({
        "token": token,
        "score_total": score_total,
        "nivel": nivel["nivel"],
        "email_sent": email_sent,
        "email_error": email_error if not email_sent else ""
    })


@app.route('/api/resultado/<token>', methods=['GET'])
def get_resultado(token):
    conn = sqlite3.connect(DB_PATH)
    c = conn.cursor()
    c.execute('SELECT nome, email, empresa, respostas, scores_sessao, score_total, nivel, created_at FROM respostas WHERE token = ?', (token,))
    row = c.fetchone()
    conn.close()

    if not row:
        return jsonify({"error": "Token não encontrado."}), 404

    nome, email, empresa, respostas_json, scores_json, score_total, nivel_num, created_at = row
    scores_sessao = json.loads(scores_json)

    nivel_info = NIVEIS[0]
    for n in NIVEIS:
        if n["nivel"] == nivel_num:
            nivel_info = n
            break

    recomendacoes = get_recomendacoes(scores_sessao)

    return jsonify({
        "token": token,
        "nome": nome,
        "email": email,
        "empresa": empresa,
        "scores_sessao": scores_sessao,
        "score_total": score_total,
        "nivel": nivel_info,
        "recomendacoes": recomendacoes,
        "created_at": created_at
    })


if __name__ == '__main__':
    init_db()
    app.run(host='0.0.0.0', port=17003, debug=False)
